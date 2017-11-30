#r "Microsoft.WindowsAzure.Storage"

using Microsoft.WindowsAzure.Storage.Blob;


public static MailTemplate Run(CloudBlockBlob inBlob, string name,TraceWriter log)
{
    log.Info($"C# Blob trigger function Processed blob\n Name:{name}");
    var content = inBlob.DownloadText();

    var item = new MailTemplate(){
        Name = name,
        Content = content,
        PartitionKey = name,
        RowKey = name

    };
    inBlob.DeleteIfExists(DeleteSnapshotsOption.IncludeSnapshots,null,null,null);
    return item;
}

public class MailTemplate {
    public string Name {get;set;}
    public string Content {get;set;}
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
}
