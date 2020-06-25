package sample;

import java.io.*;

import javafx.concurrent.Task;
import java.net.Socket;

public class SendFileTask extends Task<Void> {

    private static int port = 2100;
    private File file;

    public SendFileTask(File file) {
        this.file = file;
    }

    protected Void call() throws Exception {
        updateMessageUTF8("Inicjalizacja.");
        this.updateProgress(0L, 1L);

        try {
            Socket socket = new Socket("localhost", port);
            if (!socket.isConnected()) {
                updateMessageUTF8("Próba połączenia zakończona niepowodzeniem.");
                return null;
            }

            DataOutputStream dataOutputStream = new DataOutputStream(socket.getOutputStream());
            Throwable throwable = null;

            try {
                dataOutputStream.writeUTF(this.file.getName());
                dataOutputStream.writeLong(this.file.length());

                long startTime = System.currentTimeMillis();
                long bytesTotal = this.file.length();
                long bytesDone = 0;
                FileInputStream fileInputStream = new FileInputStream(this.file);
                byte[] buffer = new byte[512];

                while (bytesDone != bytesTotal) {
                    int read = fileInputStream.read(buffer);
                    dataOutputStream.write(buffer, 0, read);

                    bytesDone += read;
                    long elapsedTimeSeconds = (System.currentTimeMillis() - startTime) / 1000;
                    long approxTimeLeft = elapsedTimeSeconds * (bytesTotal - bytesDone) / bytesDone;

                    this.updateProgress(bytesDone, bytesTotal);
                    updateMessageUTF8("Przesyłanie pliku " + file.getName() + "\nPozostało około " +
                            approxTimeLeft + " sekund.");
                }
                updateMessageUTF8("Plik został wysłany pomyślnie.");
                this.updateProgress(1L, 1L);
            } catch (Throwable throwable1) {
                throwable = throwable1;
                throw throwable1;
            } finally {
                if (dataOutputStream != null) {
                    try {
                        dataOutputStream.close();
                    } catch (Throwable throwable1) {
                        if (throwable != null) {
                            throwable.addSuppressed(throwable1);
                        }
                    }
                }
            }
        } catch (IOException e) {
            updateMessageUTF8("Przesyłanie zakończone niepowodzeniem. " + e.getMessage());
        }
        return null;
    }

    private void updateMessageUTF8(String message) {
        byte bytes[] = message.getBytes();
        try {
            String messageUTF8 = new String(bytes, "UTF-8");
            this.updateMessage(messageUTF8);
        } catch (UnsupportedEncodingException e) {
            e.printStackTrace();
        }
    }
}