using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace ManageMLSData
{
    class Program
    {
        static void Main(string[] args)
        {
              
            string UserErrorMessage = Environment.NewLine + "Arguments are null/missing." + Environment.NewLine;
            UserErrorMessage += "Please pass Property Type as first argument & Path as second argument." + Environment.NewLine;
            UserErrorMessage += "For eg : E:\\ManageMLSData.exe \"idxcommercial\" \"C:\\MLSData\\IDXImagesCommercial\\IDXCommercial.xml\"" + Environment.NewLine;
            UserErrorMessage += "Possible values For Property Type is : vowcondo, voxcommercial, voxresidential, idxcondo, idxcommercial, idxresidential.";
            if (args.Length == 0)
            {
               // ManageMLSListDataRefresh();
                WriteLog(UserErrorMessage);
                Console.WriteLine(UserErrorMessage);
            }
            else if (args.Length == 1)
            {
                WriteLog(UserErrorMessage);
                Console.WriteLine(UserErrorMessage);
            }
            else
            {
                try
                {
                    string argument =  args[0];  //Type "IDXCondo";// 
                    string argument1 = args[1]; //Path @"C:\MlsData\IDXImagesCondo";//

                    if (argument.ToLower() == EnumValue.GetEnumDescription(EnumValue.XmlType.VowCondo))
                    {
                        UploadVOWCondo(argument1);
                    }
                    else if (argument.ToLower() == EnumValue.GetEnumDescription(EnumValue.XmlType.VoxCommercial))
                    {
                        UploadVoxCommercial(argument1);
                    }
                    else if (argument.ToLower() == EnumValue.GetEnumDescription(EnumValue.XmlType.VoxResidential))
                    {
                        UploadVoxResidential(argument1);
                        
                    }
                    else if (argument.ToLower() == EnumValue.GetEnumDescription(EnumValue.XmlType.IDXCondo))
                    {
                        UploadIDXCondo(argument1);
                        ManageMLSListDataRefresh();
                    }
                    else if (argument.ToLower() == EnumValue.GetEnumDescription(EnumValue.XmlType.IDXCommercial))
                    {
                        UploadIDXCommercial(argument1);
                        //UploadIDXCommercial(@"C:\MLSData\IDXImagesCommercial\IDXCommercial.xml");
                    }
                    else if (argument.ToLower() == EnumValue.GetEnumDescription(EnumValue.XmlType.IDXResidential))
                    {
                        UploadIDXResidential(argument1);
                        
                    }
                    else
                    {
                        UserErrorMessage = Environment.NewLine + "Type : " + argument + " is wrong." + Environment.NewLine;
                        UserErrorMessage += "Possible values are : vowcondo, voxcommercial, voxresidential, idxcondo, idxcommercial, idxresidential.";
                        WriteLog(UserErrorMessage);
                        Console.WriteLine(UserErrorMessage);
                    }
                }
                catch (Exception ex)
                {
                    string ErrorMessage = ex.Message.ToString();
                    WriteLog(ErrorMessage);
                    Console.WriteLine(Environment.NewLine + ErrorMessage);
                }

            }
        }
        public static void ManageMLSListDataRefresh()
        {
            string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase) + "\\Config.txt";
            string[] SettingList = File.ReadAllLines(path.Replace("file:\\", ""));
            foreach (var setting in SettingList)
            {
                string Action = setting.Split('=')[0].Trim();
                string ActionValue = setting.Split('=')[1].Replace(";","").Trim();
                if (Action.ToLower() == "activemls") //ActiveMLS
                {
                    UploadMlsIds(ActionValue);
                }
                else if (Action.ToLower() == "processimages") //ProcessImages
                {
                    //ProcessImages(ActionValue);
                }

            }
        }
        public static void UploadVOWCondo(string path)
        {
            try
            {
                UploadProcess(path, "VOWCondo.xml", "[dbo].[GenerateDataForVOXCondo]","VowCondoXmlForPhotos.xml");
            }
            catch (Exception ex)
            {

                string ErrorMessage = ex.Message.ToString();
                WriteLog("Error in VOWCondo function : " + ErrorMessage);
            }
        }
        public static void UploadVoxCommercial(string path)
        {
            try
            {
                UploadProcess(path, "VowCommercial.xml", "[dbo].[GenerateDataForVOXCommercial]","VowCommercialXmlForPhotos.xml");
            }
            catch (Exception ex)
            {

                string ErrorMessage = ex.Message.ToString();
                WriteLog("Error in VowCommercial function : " + ErrorMessage);
            }
        }
        public static void UploadVoxResidential(string path)
        {
            try
            {
                UploadProcess(path, "VowResidential.xml", "[dbo].[GenerateDataForVOXResidential]","VowResidentialXmlForPhotos.xml");
            }
            catch (Exception ex)
            {

                string ErrorMessage = ex.Message.ToString();
                WriteLog("Error in VowResidential function : " + ErrorMessage);
            }
        }
        public static void UploadIDXCommercial(string path)
        {
            try
            {
                UploadProcess(path, "IDXCommercial.xml", "[dbo].[GenerateDataForIDXCommercial]","IdxCommercialForPhotos.xml");
            }
            catch (Exception ex)
            {

                string ErrorMessage = ex.Message.ToString();
                WriteLog("Error in IDXCommercial function : " + ErrorMessage);
            }
        }
        public static void UploadIDXCondo(string path)
        {
            try
            {
                UploadProcess(path, "IdxCondo.xml", "[dbo].[GenerateDataForIDXCondo]","IdxCondoXmlForPhotos.xml");
            }
            catch (Exception ex)
            {

                string ErrorMessage = ex.Message.ToString();
                WriteLog("Error in IDXCondo function: " + ErrorMessage);
            }
        }
        public static void UploadIDXResidential(string path)
        {
            try
            {
                UploadProcess(path, "IDXResidential.xml", "[dbo].[GenerateDataForIDXResidential]", "IdxResidentialXmlForPhotos.xml");
            }
            catch (Exception ex)
            {

                string ErrorMessage = ex.Message.ToString();
                WriteLog("Error in IDXResidential function : " + ErrorMessage);
            }
        }
        public static void UploadProcess(string path, string xmlName, string spName,string NewFileName)
        {
            string ErrorMessageUser = "";
            try
            {
                string fullFilePath = path + "//" + xmlName;

                RefineXmlFile( path, xmlName);
                CreateCopyOfFile(path, NewFileName, xmlName);

                //Refine File
                if (File.ReadLines(fullFilePath).Any(line => line.Contains("REProperties")))
                {
                    WriteLog(fullFilePath);
                    XDocument XMLDoc = XDocument.Load(fullFilePath);
                    XElement element = (from xml1 in XMLDoc.Descendants("REProperties")
                                        select xml1).FirstOrDefault();
                    ErrorMessageUser = xmlName + " started.";
                    WriteLog(ErrorMessageUser);
                    Console.WriteLine(Environment.NewLine + ErrorMessageUser + Environment.NewLine + "Please wait....");

                    System.IO.File.WriteAllText(fullFilePath, element.ToString()
                        .Replace("<REProperties>", "")
                        .Replace("</REProperties>", ""));                        

                    ErrorMessageUser = xmlName + " done.";
                    WriteLog(ErrorMessageUser);
                    Console.WriteLine(ErrorMessageUser);

                }
                //End : Refine File

                

                //Start Store Procedure
                CommonClass clsObj = new CommonClass();
                ErrorMessageUser = spName + " started.";
                WriteLog(ErrorMessageUser);
                Console.WriteLine(Environment.NewLine + ErrorMessageUser + Environment.NewLine + "Please wait....");
                
                clsObj.ExecuteNonQuery(spName + " '" + fullFilePath + "'");

                ErrorMessageUser = spName + " done.";
                WriteLog(ErrorMessageUser);
                Console.WriteLine(ErrorMessageUser);
                //End : Start Store Procedure
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                WriteLog("Error in " + xmlName + " : " + ErrorMessage);
                Console.WriteLine(Environment.NewLine + ErrorMessage);
            }
        }
        public static void UploadMlsIds(string path)
        {
            try
            {
                ProcessFiles(path);
            }
            catch (Exception ex)
            {

                string ErrorMessage = ex.Message.ToString();
                WriteLog("Error in UploadMlsIds function : " + ErrorMessage);
            }
        }
        public static void ProcessFiles(string path)
        {
            string ErrorMessageUser = "";
            try
            {
                //List<string> MLSActiveFileList = new List<string>();
                //MLSActiveFileList.Add("ActiveIdxCommercial.txt");
                //MLSActiveFileList.Add("ActiveIdxCondo.txt");
                //MLSActiveFileList.Add("ActiveIdxResidential.txt");
                //MLSActiveFileList.Add("ActiveVowCommercial.txt");
                //MLSActiveFileList.Add("ActiveVowCondo.txt");
                //MLSActiveFileList.Add("ActiveVowResidential.txt");

                //List<string> MlsAddedList = new List<string>();
                string QStr = "";

                ErrorMessageUser = "Processing file.";
                WriteLog(ErrorMessageUser);
                Console.WriteLine(Environment.NewLine + ErrorMessageUser + Environment.NewLine + "Please wait....");

                ////foreach (var MLSActiveFile in MLSActiveFileList)
                ////{
                ////    string NewPath = path + "/" + MLSActiveFile;
                ////    var webRequest = System.Net.WebRequest.Create(NewPath);

                ////    using (var response = webRequest.GetResponse())
                ////    using (var content = response.GetResponseStream())
                ////    using (var reader = new StreamReader(content))
                ////    {
                ////        var strContent = reader.ReadToEnd();
                ////        string[] Lines = strContent.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                ////        foreach (var line in Lines)
                ////        {
                ////            string[] MlsId = line.Split(' ');
                ////            if (MlsId.Length > 0)
                ////            {
                ////                string MlsIdMain = MlsId[0].Trim();
                ////                if (MlsIdMain != "")
                ////                {
                ////                    if (!MlsAddedList.Contains(MlsIdMain))
                ////                    {
                ////                        QStr += "INSERT INTO [ActiveMLSIDS]([MLSID])VALUES('" + MlsIdMain + "');";
                ////                        MlsAddedList.Add(MlsIdMain);
                ////                    }
                ////                }
                ////            }
                ////        }
                ////    }
                ////}

                //foreach (string file in Directory.EnumerateFiles(path, "*.txt"))
                //{
                //    string[] Lines = File.ReadAllLines(file);
                //    foreach (var line in Lines)
                //    {
                //        string[] MlsId = line.Split(' ');
                //        if (MlsId.Length > 0)
                //        {
                //            string MlsIdMain = MlsId[0].Trim();
                //            if (MlsIdMain != "")
                //            {
                //                if (!MlsAddedList.Contains(MlsIdMain))
                //                {
                //                    QStr += "INSERT INTO [ActiveMLSIDS]([MLSID])VALUES('" + MlsIdMain + "');";
                //                    MlsAddedList.Add(MlsIdMain);
                //                }
                //            }
                //        }
                //    }
                //}

                CommonClass clsObj = new CommonClass();
                clsObj.ExecuteNonQuery("UpdateActiveProperties");
                ////if (MlsAddedList.Count() > 0)
                ////{
                ////    clsObj.ExecuteNonQuery("TRUNCATE TABLE [ActiveMLSIDS]");

                //Remove left right space from mls columns.
                ////QStr += "UPDATE[ActiveMLSIDS] SET MLSID = SUBSTRING(mlsiD, 0, LEN(mlsiD))WHERE mlsiD LIKE '%A';";

                ////QStr += "update [ActiveMLSIDS] set MLSID=LTRIM(RTRIM(REPLACE(REPLACE(REPLACE(MLSID, CHAR(10), ''), CHAR(13), ''), CHAR(9), '')));";

                ////QStr += " UPDATE dbo.[ActiveMLSIDS] SET mlsid = REPLACE(mlsid, ',', ''); ";




                ////QStr += "update PropertyData set MLS = LTRIM(RTRIM(REPLACE(REPLACE(REPLACE(MLS, CHAR(10), ''), CHAR(13), ''), CHAR(9), '')));";
                ////QStr += "update PropertyData_Comm            set MLS = LTRIM(RTRIM(REPLACE(REPLACE(REPLACE(MLS, CHAR(10), ''), CHAR(13), ''), CHAR(9), '')));";
                ////QStr += "update PropertyData_Comm_VOX        set MLS = LTRIM(RTRIM(REPLACE(REPLACE(REPLACE(MLS, CHAR(10), ''), CHAR(13), ''), CHAR(9), '')));";
                ////QStr += "update PropertyData_Condo           set MLS = LTRIM(RTRIM(REPLACE(REPLACE(REPLACE(MLS, CHAR(10), ''), CHAR(13), ''), CHAR(9), '')));";
                ////QStr += "update PropertyData_Condo_Vox       set MLS = LTRIM(RTRIM(REPLACE(REPLACE(REPLACE(MLS, CHAR(10), ''), CHAR(13), ''), CHAR(9), '')));";
                ////QStr += "update PropertyData_Vox_Residential set MLS = LTRIM(RTRIM(REPLACE(REPLACE(REPLACE(MLS, CHAR(10), ''), CHAR(13), ''), CHAR(9), '')));";
                //end

                //Start Insert Procedure
                QStr += "DELETE FROM PropertyData where MLS NOT IN(SELECT MLSID FROM [ActiveMLSIDS]);";
                QStr += "DELETE FROM PropertyData where MLS NOT IN(SELECT MLSID FROM [ActiveMLSIDS]);";
                QStr += "DELETE FROM PropertyData where MLS NOT IN(SELECT MLSID FROM [ActiveMLSIDS]);";
                QStr += "DELETE FROM PropertyData_Comm where MLS NOT IN(SELECT MLSID FROM [ActiveMLSIDS]);";
                QStr += "DELETE FROM PropertyData_Comm_VOX where MLS NOT IN(SELECT MLSID FROM [ActiveMLSIDS]);";
                QStr += "DELETE FROM PropertyData_Condo where MLS NOT IN(SELECT MLSID FROM [ActiveMLSIDS]);";
                QStr += "DELETE FROM PropertyData_Condo_Vox where MLS NOT IN(SELECT MLSID FROM [ActiveMLSIDS]);";
                QStr += "DELETE FROM PropertyData_Vox_Residential where MLS NOT IN(SELECT MLSID FROM [ActiveMLSIDS]);";


                ////ErrorMessageUser = "Insert started.";
                ////WriteLog(ErrorMessageUser);
                ////Console.WriteLine(Environment.NewLine + ErrorMessageUser + Environment.NewLine + "Please wait....");

                ////clsObj.ExecuteNonQuery(QStr);

                ErrorMessageUser = "Insert done.";
                    WriteLog(ErrorMessageUser);
                    Console.WriteLine(ErrorMessageUser);
                    //End : Start Insert Procedure
                //}
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                WriteLog("Error in Insert ActiveMLSIDS : " + ErrorMessage);
                Console.WriteLine(Environment.NewLine + ErrorMessage);
            }
        }
        public static void ProcessImages(string path)
        {
            string ErrorMessageUser = "";
            try
            {
                ErrorMessageUser = "Process Images started.";
                WriteLog(ErrorMessageUser);
                Console.WriteLine(Environment.NewLine + ErrorMessageUser + Environment.NewLine + "Please wait....");

                //Note: e.g => 'http://169.45.133.92:8060/IDXResidential/Photo' + MLS + '-1.jpeg'
                List<string> FolderName = new List<string>();
                FolderName.Add("IDXResidential");
                FolderName.Add("IDXImagesCommercial");
                FolderName.Add("VoxCommercial");
                FolderName.Add("IDXImagesCondo");
                FolderName.Add("VoxCondo");
                FolderName.Add("VoxResidential");

                CommonClass clsObj = new CommonClass();
                System.Data.DataTable dt = new System.Data.DataTable();
                dt = clsObj.GetDataSet("SELECT MLSID FROM [ActiveMLSIDS]").Tables[0];
                foreach (var folder in FolderName)
                {
                    string NewPath = path + "\\" + folder;
                    if (Directory.Exists(NewPath))
                    {
                        string[] filePaths = Directory.GetFiles(NewPath, "*.jpeg", SearchOption.AllDirectories);
                        foreach (var file in filePaths)
                        {
                            try
                            {
                                string FileName = Path.GetFileName(file);
                                string MLSId = FileName.Replace("Photo", "").Split('-')[0].Trim();
                                System.Data.DataRow[] dr = dt.Select("MLSID='" + MLSId + "'");
                                if (dr.Length == 0) //MLSID Not Found in DB. So Delete its image from folder.
                                {
                                    if (File.Exists(file))
                                    {
                                        File.Delete(file);
                                    }

                                }
                            }
                            catch (Exception ex)
                            {
                                string ErrorMessage = ex.Message.ToString();
                                WriteLog("Error in Process Images file : " + file);
                                Console.WriteLine(Environment.NewLine + ErrorMessage);
                            }
                            
                        }
                    }
                }

                ErrorMessageUser = "Process Images done.";
                WriteLog(ErrorMessageUser);
                Console.WriteLine(ErrorMessageUser);
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                WriteLog("Error in Process Images : " + ErrorMessage);
                Console.WriteLine(Environment.NewLine + ErrorMessage);
            }

        }
        public static void WriteLog(string Message)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("==============================================================================" + Environment.NewLine);
            sb.Append("Event occurred on : " + DateTime.Now + Environment.NewLine);
            sb.Append(Message + Environment.NewLine);
            sb.Append("==============================================================================" + Environment.NewLine);

            string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase) + "\\ErrorLog.txt";

            System.IO.File.AppendAllText(path.Replace("file:\\", ""), sb.ToString());
            //System.IO.File.WriteAllText(path.Replace("file:\\", ""), sb.ToString());
        }

        public static void CreateCopyOfFile(string path, string NewfileName, string OldFileName)
        {
            
            try
            {
                // Use Path class to manipulate file and directory paths.
                string sourceFile = System.IO.Path.Combine(path, OldFileName);
                string destFile = System.IO.Path.Combine(path, NewfileName);

                // To copy a folder's contents to a new location:
                // Create a new target folder, if necessary.
                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }

                if (File.Exists(path + "/" + NewfileName))
                {
                    File.Delete(path + "/" + NewfileName);
                }

                // To copy a file to another location and 
                // overwrite the destination file if it already exists.
                System.IO.File.Copy(sourceFile, destFile, true);
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                WriteLog("Error in CreateCopyOfFile function: " + ErrorMessage);
            }
           
        }

        public static void RefineXmlFile(string path, string xmlfilename)
        {
            try
            {

                var lines = File.ReadAllLines(path+"//"+ xmlfilename);
                var xml = new StringBuilder();
                var isXml = false;
                foreach (var line in lines)
                {


                    if (line.Contains("<!DOCTYPE RETS SYSTEM "))
                        isXml = true;
                    if (isXml)
                    {
                        xml.Append(line.Replace("<!DOCTYPE RETS SYSTEM \"RETS-20041001.dtd\">", ""));
                        isXml = false;
                    }
                    else if (line.Contains("<RETS ReplyCode="))
                    {
                        isXml = true;
                        if (isXml)
                            xml.Append(line.Replace("<RETS ReplyCode=\"0\" ReplyText=\"\">", ""));
                        isXml = false;
                    }
                    else if (line.Contains("<REData>"))
                    {
                        isXml = true;
                        if (isXml)
                            xml.Append(line.Replace("<REData>", ""));
                        isXml = false;
                    }
                    else if (line.Contains("</RETS>"))
                    {
                        isXml = true;
                        if (isXml)
                            xml.Append(line.Replace("</RETS>", ""));
                        isXml = false;
                    }
                    else if (line.Contains("</REData>"))
                    {
                        isXml = true;
                        if (isXml)
                            xml.Append(line.Replace("</REData>", ""));
                        isXml = false;
                    }
                    else if (line.Contains("<MAXROWS/>"))
                    {
                        isXml = true;
                        if (isXml)
                            xml.Append(line.Replace("<MAXROWS/>", ""));
                        isXml = false;
                    }
                    else if (line.Contains("<?xml version="))
                    {
                        isXml = true;
                        if (isXml)
                            xml.Append(line.Replace("<?xml version=\"1.0\" standalone=\"no\"?>", ""));
                        isXml = false;
                    }
                    else
                    {
                        xml.Append(line);
                    }

                }
                System.IO.File.WriteAllText(path, xml.ToString());
            }
            catch (Exception ex)
            {

            }


        }
    }
}
