import java.io.File;
import java.text.SimpleDateFormat;
import java.util.Date;

public abstract class DiskElement implements Comparable<DiskElement> {
    protected File file;
    protected String path;
    protected String name;
    protected Date lastModified;
    protected final SimpleDateFormat format = new SimpleDateFormat("yyyy-MM-dd");

    protected abstract int maxLineLength(int depth);
    protected abstract void print(int depth, int lineLength);

    public void print() { print(0, maxLineLength(0)); }

    public String getPath(){
        return this.path;
    }

    public int compareTo(DiskElement other) {
        return this.name.compareTo(other.name);
    }
}