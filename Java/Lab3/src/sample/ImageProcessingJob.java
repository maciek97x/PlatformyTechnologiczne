package sample;

import javafx.application.Platform;
import javafx.beans.property.DoubleProperty;
import javafx.beans.property.SimpleStringProperty;
import javafx.beans.property.StringProperty;
import javafx.beans.value.ObservableValue;
import javafx.concurrent.Task;

import javax.imageio.ImageIO;
import java.awt.*;
import java.awt.image.BufferedImage;
import java.io.File;
import java.io.IOException;
import java.nio.file.Path;
import java.nio.file.Paths;

public class ImageProcessingJob extends Task<Void> {
    public File sourceFile;
    public File outputDirectory;
    public DoubleProperty progress;
    public SimpleStringProperty status;

    public File getFile() {
        return sourceFile;
    }
    public ImageProcessingJob(File file) {
        sourceFile = file;
        outputDirectory = null;

        updateMessage("waiting");
    }

    public Void call() throws Exception {
        if (outputDirectory == null) {
            updateMessage("output dir is null");
            return null;
        } else if (sourceFile == null) {
            updateMessage("file is null");
            return null;
        }
        updateMessage("processing...");
        updateProgress(0, 1);
        try {
            //wczytanie oryginalnego pliku do pamięci
            BufferedImage original = ImageIO.read(sourceFile);

            //przygotowanie bufora na grafikę w skali szarości
            BufferedImage grayscale = new BufferedImage(original.getWidth(), original.getHeight(), original.getType());
            //przetwarzanie piksel po pikselu
            for (int i = 0; i < original.getWidth(); i++) {
                for (int j = 0; j < original.getHeight(); j++) {
                    //pobranie składowych RGB
                    int red = new Color(original.getRGB(i, j)).getRed();
                    int green = new Color(original.getRGB(i, j)).getGreen();
                    int blue = new Color(original.getRGB(i, j)).getBlue();
                    //obliczenie jasności piksela dla obrazu w skali szarości
                    int luminosity = (int) (0.21*red + 0.71*green + 0.07*blue);
                    //przygotowanie wartości koloru w oparciu o obliczoną jaskość
                    int newPixel = new Color(luminosity, luminosity, luminosity).getRGB();
                    //zapisanie nowego piksela w buforze
                    grayscale.setRGB(i, j, newPixel);
                }
                //obliczenie postępu przetwarzania jako liczby z przedziału [0, 1]
                double progress = (1.0 + i) / original.getWidth();
                //aktualizacja własności zbindowanej z paskiem postępu w tabeli
                Platform.runLater(() -> updateProgress(progress, 1));
            }
            //przygotowanie ścieżki wskazującej na plik wynikowy
            Path outputPath = Paths.get(outputDirectory.getAbsolutePath(), sourceFile.getName());

            //zapisanie zawartości bufora do pliku na dysku
            ImageIO.write(grayscale, "jpg", outputPath.toFile());
        } catch (IOException ex) {
            //translacja wyjątku
            throw new RuntimeException(ex);
        }
        updateMessage("completed");
        return null;
    }

}
