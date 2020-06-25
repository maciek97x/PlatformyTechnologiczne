package sample;

import java.io.File;
import java.io.Serializable;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import javafx.concurrent.Task;
import javafx.event.ActionEvent;
import javafx.fxml.FXML;
import javafx.stage.FileChooser;
import javafx.scene.control.Label;
import javafx.scene.control.ProgressBar;
import javafx.scene.control.Button;

public class Controller implements Serializable {
    @FXML private Label statusLabel;
    @FXML private ProgressBar progressBar;
    @FXML private Button uploadButton;
    private ExecutorService executorService = Executors.newSingleThreadExecutor();

    public Controller() {}

    @FXML
    private void buttonClick(ActionEvent event) {
        File file = (new FileChooser()).showOpenDialog(null);
        if (file != null && file.exists()) {
            Task<Void> sendFileTask = new SendFileTask(file);

            statusLabel.textProperty().bind(sendFileTask.messageProperty());
            progressBar.progressProperty().bind(sendFileTask.progressProperty());
            uploadButton.disableProperty().bind(sendFileTask.runningProperty());
            executorService.submit(sendFileTask);
        }
    }
}