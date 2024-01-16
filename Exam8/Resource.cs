namespace Exam8
{
    public enum Type { Folder, File }
    internal class Resource
    {
        public string Name {  get; set; }
        public Type ResourceType {  get; set; }
        public long Size { get; set; }
        public string LastAccess {  get; set; }
    }
}
