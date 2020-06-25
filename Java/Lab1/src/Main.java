import java.io.File;

public class Main {
    public static void main(String[] args) {
        if (args.length == 0) {
            printHelp();
            return;
        }
        String path = null;
        int sort = 0;
        boolean recursive = false;

        for (int i = 0; i < args.length; ++i) {
            String arg = args[i];
            if (arg.equals("-p")) {
                if (i + 1 < args.length) {
                    path = args[i + 1];
                }
            }
            else if (arg.equals("-r")) {
                recursive = true;
            }
            else if (arg.equals("-a")) {
                sort = sort | 1;
            }
            else if (arg.equals("-s")) {
                sort = sort | 2;
            }
            else if (arg.equals("-h")) {
                printHelp();
                return;
            }
        }
        if (path == null) {
            System.out.println("Podaj ścieżkę.");
            return;
        }

        if (sort == 0) {
            System.out.println("Wybierz metodę sortowania.");
            return;
        }

        if (sort == 3) {
            System.out.println("Wybierz jedną metodę sortowania.");
            return;
        }

        File tmp = new File(path);
        if (!tmp.exists()){
            System.out.println("Podana ścieżka nie istnieje.");
            return;
        }

        DiskDirectory dir = new DiskDirectory(path, sort, recursive);
        dir.print();
    }

    private static void printHelp() {
        System.out.println("Argumenty programu:");
        System.out.println("    -p <ścieżka> wybrana ścieżka,");
        System.out.println("    -a sortowanie alfabetyczne,");
        System.out.println("    -s sortowanie po rozmiarze,");
        System.out.println("    -r przeszukiwanie rekurencyjne.");
    }
}
