using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace CommonFoundation.Common
{
   public class FileOperate
    {
       /// <summary>
       /// 生成Kbase数据库导入数据需要的REC文件
       /// </summary>
       /// <param name="str"></param>
       public static void WriteREC(string str)
       {
           string logDirPath = @"E:\CNKI\MyData";//目录
           string logFilePath = string.Format("{0}\\Test_{1}.txt", logDirPath, System.DateTime.Now.ToString("yyyy-MM-dd"));//日志文件全路径
           StreamWriter sw;
           //加锁
           object objLock = new object();
           lock (objLock)
           {
               //检查是否有该路径  没有就创建
               if (!System.IO.Directory.Exists(logDirPath))
               {
                   System.IO.Directory.CreateDirectory(logDirPath);//创建文件目录
               }

               using (sw = new StreamWriter(logFilePath, true, Encoding.UTF8))
               {
                   sw.WriteLine(str);
               }
           }
       }
       /// <summary>
       /// 创建路径
       /// </summary>
       /// <param name="path"></param>
       public static void CreatePath(string path)
       {
           //检查是否有该路径,没有就创建
           if (!string.IsNullOrEmpty(path) && !Directory.Exists(path))
           {
               Directory.CreateDirectory(path);//创建文件目录
           }
       }
       private static List<string> dirPathList = null;
       //获得某一文件夹下的所有可访问的子文件夹
       public static List<string> GetAllEnablePath(string folderPath)
       {
           List<string> list = null;
           GetAllChildDirectory(folderPath);
           if (dirPathList != null && dirPathList.Count > 0)
           {
               list = new List<string>();
               foreach (string path in dirPathList)
               {
                   try
                   {
                       DirectorySecurity secu = new DirectorySecurity(path, AccessControlSections.Access);
                       if (!secu.AreAccessRulesProtected)
                       {
                           list.Add(path);
                       }
                   }
                   catch
                   {
                       list = null;
                       break;
                   }
               }
           }
           return list;
       }
       //获得某一文件夹下的所有子文件夹(递归)
       private static void GetAllChildDirectory(string folderPath)
       {
           string[] dirs = GetDirPath(folderPath);
           if (dirs != null)
           {
               dirPathList = new List<string>();
               foreach (string dir in dirs)
               {
                   dirPathList.Add(dir);
                   GetAllChildDirectory(dir);
               }
           }
       }
       //获得某一文件夹下的子文件夹（一级）
       private static string[] GetDirPath(string dirName)
       {
           try
           {
               return Directory.GetDirectories(dirName);
           }
           catch
           {
               return null;
           }
       }

       private static readonly object ObjLock = new object();
       /// <summary>
       /// 创建文件并写入数据
       /// </summary>
       /// <param name="message"></param>
       /// <param name="logFullPath"></param>
       public static void WriteFileLog(string message, string logFullPath)
       {
           lock (ObjLock)
           {
               var path = Path.GetDirectoryName(logFullPath);
               if (path != null)
               {
                   //创建文件目录
                   var directoryInfo = new FileInfo(logFullPath).Directory;
                   if (directoryInfo != null)
                       Directory.CreateDirectory(directoryInfo.FullName);
                   using (var sw = File.AppendText(logFullPath))
                   {
                       sw.Write(message);
                   }
               }
           }
       }
    }
}
