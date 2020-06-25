package sample;

import javafx.application.Application;
import javafx.fxml.FXMLLoader;
import javafx.scene.Parent;
import javafx.stage.Stage;
import javafx.scene.Scene;

public class Main extends Application {
    public Main() {
    }

    public void start(Stage stage) throws Exception {
        Parent root = FXMLLoader.load(this.getClass().getResource("window.fxml"));
        stage.setTitle("Lab2");
        stage.setScene(new Scene(root, 320.0D, 240.0D));
        stage.show();
    }

    @Override
    public void stop(){
        System.exit(0);
    }
    public static void main(String[] args) {
        launch(args);
    }
}