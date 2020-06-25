package sample;

import javafx.application.Platform;
import javafx.beans.property.SimpleStringProperty;
import javafx.beans.value.ObservableValue;
import javafx.collections.FXCollections;
import javafx.collections.ObservableList;
import javafx.concurrent.Task;
import javafx.event.ActionEvent;
import javafx.fxml.FXML;
import javafx.fxml.Initializable;
import javafx.scene.Node;
import javafx.scene.control.*;
import javafx.scene.control.cell.ProgressBarTableCell;
import javafx.stage.DirectoryChooser;
import javafx.stage.FileChooser;
import javafx.stage.Window;

import java.io.File;
import java.net.URL;
import java.util.ArrayList;
import java.util.List;
import java.util.ResourceBundle;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.concurrent.ForkJoinPool;
import java.util.concurrent.TimeUnit;

public class Controller implements Initializable {
    @FXML private TableView<ImageProcessingJob> imagesTableView;
    @FXML private TableColumn<ImageProcessingJob, String> imageNameColumn;
    @FXML private TableColumn<ImageProcessingJob, Double> progressColumn;
    @FXML private TableColumn<ImageProcessingJob, String> statusColumn;

    @FXML private Button selectFilesButton;
    @FXML private Button selectOutputDirButton;
    @FXML private Button convertFilesButton;
    @FXML private Label statusLabel;

    @FXML private Spinner<Integer> numberOfThreadsSpinner;
    @FXML private ComboBox<String> convertMethodComboBox;

    private File outputDirectory = null;
    ObservableList<ImageProcessingJob> jobs;

    ExecutorService executor = Executors.newSingleThreadExecutor();

    @Override
    public void initialize(URL url, ResourceBundle rb) {
        imageNameColumn.setCellValueFactory(p -> new SimpleStringProperty(p.getValue().getFile().getName()));
        statusColumn.setCellValueFactory(p -> p.getValue().messageProperty());
        progressColumn.setCellFactory(ProgressBarTableCell.<ImageProcessingJob>forTableColumn());
        progressColumn.setCellValueFactory(p -> p.getValue().progressProperty().asObject());

        numberOfThreadsSpinner.valueProperty().addListener((obs, oldValue, newValue) -> selectConvertMethod());

        jobs = FXCollections.observableList(new ArrayList<>());
        imagesTableView.setItems(jobs);
    }

    @FXML
    private void selectFiles(ActionEvent event) {
        FileChooser fileChooser = new FileChooser();
        fileChooser.getExtensionFilters().add(
                new FileChooser.ExtensionFilter("JPG images", "*.jpg"));
        List<File> selectedFiles = fileChooser.showOpenMultipleDialog(null);
        if (!(selectedFiles == null || selectedFiles.isEmpty())) {
            jobs.clear();
            selectedFiles.stream().forEach(file -> { jobs.add(new ImageProcessingJob(file)); });
        }
    }

    @FXML
    void processFiles(ActionEvent event) {
        new Thread(this::backgroundJob).start();
    }

    //metoda uruchamiana w tle (w tej samej klasie)
    private void backgroundJob() {
        long startTime = System.currentTimeMillis();
        Platform.runLater(() -> {
            disableControls(true);
            statusLabel.setText("Converting...");
        });

        jobs.stream().forEach(job -> {
            job.outputDirectory = outputDirectory;
            executor.submit(job);
        });

        try {
            executor.shutdown();
            executor.awaitTermination(1, TimeUnit.HOURS);

            long elapsedTime = System.currentTimeMillis() - startTime;
            Platform.runLater(() -> {
                statusLabel.setText("Converted in " + elapsedTime + "ms.");
                disableControls(false);
                convertFilesButton.setDisable(true);
                selectConvertMethod();
            });

        } catch (InterruptedException e) {
            Platform.runLater(() -> { statusLabel.setText("Error."); });
        }
    }

    @FXML
    void selectOutputDir(ActionEvent event) {
        DirectoryChooser directoryChooser = new DirectoryChooser();
        Window stage = ((Node)event.getSource()).getScene().getWindow();
        outputDirectory = directoryChooser.showDialog(stage);
        if (outputDirectory != null) {
            convertFilesButton.setDisable(false);
        }
    }

    @FXML
    void selectConvertMethod() {
        String type = convertMethodComboBox.getValue();
        numberOfThreadsSpinner.setDisable(!type.equals("Selected number of threads"));
        switch (type){
            case "Sequential":
                executor = Executors.newSingleThreadExecutor();
                break;
            case "Default number of threads":
                executor = new ForkJoinPool();
                break;
            case "Selected number of threads":
                executor = Executors.newFixedThreadPool(numberOfThreadsSpinner.getValue());
                break;
        }
    }

    void disableControls(boolean disable) {
        numberOfThreadsSpinner.setDisable(disable);
        convertMethodComboBox.setDisable(disable);
        selectFilesButton.setDisable(disable);
        selectOutputDirButton.setDisable(disable);
        convertFilesButton.setDisable(disable);
    }
}
