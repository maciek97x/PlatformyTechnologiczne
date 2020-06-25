import java.io.File;
import java.util.*;

public class DiskDirectory extends DiskElement {
    private Set<DiskElement> children;
    private int sort;
    private Boolean recursive;

    public DiskDirectory(String path, int sort, Boolean recursive) {
        this.sort = sort;
        this.recursive = recursive;
        switch(sort) {
            case 1:
                children = new TreeSet<>();
                break;
            case 2:
                children = new TreeSet<>((o1, o2) -> o1.file.length() > o2.file.length() ? 1 : -1);
                break;
            default:
                System.err.println("Wybrano błędne sortowanie.");
                return;
        }

        this.path = path;
        this.file = new File(path);
        this.name = file.getName();
        this.lastModified = new Date(file.lastModified());

        File[] files = this.file.listFiles();

        for (File child : Objects.requireNonNull(files)) {
            if (child.isDirectory() && recursive) {
                DiskDirectory diskDirectory = new DiskDirectory(child.getPath(), sort, recursive);
                this.children.add(Objects.requireNonNull(diskDirectory));
            }
            else {
                DiskFile diskFile = new DiskFile(child.getPath());
                this.children.add(Objects.requireNonNull(diskFile));
            }
        }
    }

    protected int maxLineLength(int depth) {
        int lineLength = depth + this.name.length() + 13;
        for (DiskElement diskElement : children) {
            if (diskElement.getClass() == DiskDirectory.class) {
                DiskDirectory diskDirectory = new DiskDirectory(diskElement.getPath(), this.sort, this.recursive);
                int diskDirectoryLineLength = diskDirectory.maxLineLength(depth + 1);
                if (diskDirectoryLineLength > lineLength) {
                    lineLength = diskDirectoryLineLength;
                }
            }
            else {
                DiskFile diskFile = new DiskFile(diskElement.getPath());
                int diskFileLineLength = diskFile.maxLineLength(depth + 1);
                if (diskFileLineLength > lineLength) {
                    lineLength = diskFileLineLength;
                }
            }

        }
        return lineLength;
    }

    protected void print(int depth, int lineLength) {
        for (int i = 0; i < depth + 1; ++i) {
            System.out.print("-");
        }
        System.out.print(this.name);
        for (int i = 0; i < lineLength - depth - this.name.length() - 13; ++i) {
            System.out.print(" ");
        }
        System.out.print(" K " + format.format(this.lastModified));
        System.out.println();

        for (DiskElement diskElement : children) {
            if (diskElement.getClass() == DiskDirectory.class) {
                DiskDirectory diskDirectory = new DiskDirectory(diskElement.getPath(), this.sort, this.recursive);
                diskDirectory.print(depth + 1, lineLength);
            }
        }
        for (DiskElement diskElement : children) {
            if (diskElement.getClass() == DiskFile.class) {
                DiskFile diskFile = new DiskFile(diskElement.getPath());
                diskFile.print(depth + 1, lineLength);
            }
        }
    }
}