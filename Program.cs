// See https://aka.ms/new-console-template for more information
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


using IHost host = Host.CreateApplicationBuilder(args).Build();

IConfiguration config = host.Services.GetRequiredService<IConfiguration>();

string? sourceRootDirectory = config.GetValue<string>("sourceFiles:RootDirectory");
string? fromUserEmail = config.GetValue<string>("sourceFiles:sourceEma5ilAddr");
string? targetDirectory = config.GetValue<string>("targetFiles:Directory");
string[]? targetFileNames = config.GetSection("targetFiles:FileNames").Get<string[]>(); 
string? targetUserEmail = config.GetValue<string>("targetFiles:targetUserEmailAddr");
string? targetUserName = config.GetValue<string>("targetFiles:targetUserName");
string? zipDir = config.GetValue<string>("targetFiles:targetZipDir");
//string? apiKey = config.GetValue<string>("apiKey");
var fileCreateTime = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second).TotalSeconds.ToString();

zipDir += "\\"+targetUserName +"_"+ fileCreateTime;
targetDirectory +="\\"+ targetUserName +"_"+ fileCreateTime;

//clean up targetFileNames.
if(targetFileNames != null){
    for(var i = 0 ; i < targetFileNames.Length ; i++){
        targetFileNames[i] = Path.GetFileNameWithoutExtension(targetFileNames[i]);
    }
}

try{
    AutoZip.LookUpFile(sourceRootDirectory, targetDirectory, targetFileNames);
    //AutoZip.SendZipFiles(fromUserEmail, targetUserEmail, apiKey, AutoZip.ZipFiles(targetDirectory, targetUserName));
    var zipFileAddr = AutoZip.ZipFiles(targetDirectory, zipDir);
    Console.WriteLine(zipFileAddr);
    Environment.Exit(0);
}catch(Exception ex)
{
    Console.WriteLine(ex.Message);
}


host.Run();


