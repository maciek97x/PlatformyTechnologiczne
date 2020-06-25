import java.io.File;
import java.util.Date;

public class DiskFile extends DiskElement {
    public DiskFile(String path){
        this.path = path;
        this.file = new File(path);
        this.name = file.getName();
        this.lastModified = new Date(file.lastModified());
    }

    protected int maxLineLength(int depth) {
        return depth + this.name.length() + 13;
    }

    protected void print(int depth, int lineLength){
        for (int i = 0; i < depth + 1; ++i) {
            System.out.print("-");
        }
        System.out.print(this.name);
        for (int i = 0; i < lineLength - depth - this.name.length() - 13; ++i) {
            System.out.print(" ");
        }
        System.out.print(" P " + format.format(this.lastModified));
        System.out.println();
    }
}