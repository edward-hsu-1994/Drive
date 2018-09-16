export class FileNode {
  public name: string;
  public children: FileNode[];
  public relativePath?: string;
  public selected?: boolean;
  public isRoot: boolean;
}
