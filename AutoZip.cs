using System.IO.Compression;
using System.IO;
using SendGrid;
using SendGrid.Helpers.Mail;

public static class AutoZip
{
    public static void LookUpFile(string? sourceDirectoryName, string? targetDirectory, string[]? targetFileNames)
    {
        try{
            if(sourceDirectoryName == null) throw new Exception("source directory does not exist");
            DirectoryInfo sourceDirectoryInfo = new DirectoryInfo(sourceDirectoryName);
            if(targetDirectory == null) throw new Exception("Target directory does not exist");
            DirectoryInfo targetDirectoryInfo = new DirectoryInfo(targetDirectory);

            Recursive(sourceDirectoryInfo, targetDirectoryInfo,targetFileNames);

        }catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public static string ZipFiles(string? targetDir, string? zipDir)
    {
        if( targetDir == null) throw new Exception("target directory does not exist");
        try{
            string uniqueZipDir = zipDir+".zip"; 
            DirectoryInfo targetDirInfo = new DirectoryInfo(targetDir) ; 

            if(targetDirInfo.Exists == false && targetDirInfo.GetFiles().Length == 0) throw new Exception("folder is empty, look up file , copy file failed");

            ZipFile.CreateFromDirectory(targetDir,  uniqueZipDir);
            return uniqueZipDir;
        }catch(Exception ex)
        {
            throw ex;
        }
    }

    public static void SendZipFiles(string? fromEmailAddr, string? targetEmailAddr, string? apiKey, string? targetZipFilePath)
    {
        if(targetZipFilePath == null) throw new Exception("target zip folder is failed");

        var client = new SendGridClient(apiKey);
        var from = new EmailAddress(fromEmailAddr);
        var subject = "Please see attached zips files";
        var to = new EmailAddress(targetEmailAddr);
        var body = "Please see attached zip files. Thank you very much, Best regards";
        var msg = MailHelper.CreateSingleEmail(from,to,subject,body,"");
        var bytes = File.ReadAllBytes(targetZipFilePath);
        var file = Convert.ToBase64String(bytes);
        var response = client.SendEmailAsync(msg).ConfigureAwait(false);
    }

    private static void Recursive(DirectoryInfo sourceDirectoryName, DirectoryInfo targetDirectoryName, string[]? targetFileNames)
    {
        if(sourceDirectoryName.Exists == false) throw new DirectoryNotFoundException($"Source directory not found : {sourceDirectoryName.FullName}");

        try{
            var subDir = sourceDirectoryName.GetDirectories();

            if(subDir.Length > 0)
            {
                foreach(var aSubDir in subDir)
                {
                    Recursive(aSubDir, targetDirectoryName, targetFileNames);
                }
            }

            var files = sourceDirectoryName.GetFiles();
            if(files.Length > 0)
            {
                foreach(var aFile in files)
                {
                    if(targetFileNames != null && targetFileNames.Contains(Path.GetFileNameWithoutExtension(aFile.Name)))
                    {
                        CopyFileToTargetDir(aFile, targetDirectoryName);
                    }
                }
            }
        }catch(Exception ex)
        {
            throw ex;
        }
    }

    private static void CopyFileToTargetDir(FileInfo aFile, DirectoryInfo targetDir)
    {
        if(targetDir.Exists == false)
        {
            targetDir.Create();
        }
        try{
            aFile.CopyTo(targetDir.FullName+"\\"+aFile.Name,true);
        }catch(Exception ex){
            throw ex;
        }
    }
}