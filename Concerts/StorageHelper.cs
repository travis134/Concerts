using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Xml;
using System.Xml.Serialization;

namespace Concerts
{
    public class StorageHelper<T>
    {
        private String fileName;
        private String lastModifiedFileName;

        public StorageHelper(String fileName)
        {
            this.fileName = fileName;
        }

        public StorageHelper(String fileName, String lastModifiedFileName)
        {
            this.fileName = fileName;
            this.lastModifiedFileName = lastModifiedFileName;
        }

        public Boolean Exists()
        {
            IsolatedStorageFile isoStorage = IsolatedStorageFile.GetUserStoreForApplication();
            return isoStorage.FileExists(this.fileName);
        }

        public void Delete()
        {
            IsolatedStorageFile isoStorage = IsolatedStorageFile.GetUserStoreForApplication();
            isoStorage.DeleteFile(this.fileName);
        }

        public List<T> Load()
        {
            List<T> genericList = new List<T>();
            TextReader reader = null;
            try
            {
                IsolatedStorageFile isoStorage = IsolatedStorageFile.GetUserStoreForApplication();
                IsolatedStorageFileStream file = isoStorage.OpenFile(this.fileName, FileMode.OpenOrCreate);
                reader = new StreamReader(file);
                XmlSerializer xs = new XmlSerializer(typeof(List<T>));
                genericList.AddRange((List<T>)xs.Deserialize(reader));
                reader.Close();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Dispose();
            }
            return genericList;
        }

        public void Save(List<T> genericList)
        {
            TextWriter writer = null;
            try
            {
                IsolatedStorageFile isoStorage = IsolatedStorageFile.GetUserStoreForApplication();
                IsolatedStorageFileStream file = isoStorage.OpenFile(this.fileName, FileMode.Create);
                writer = new StreamWriter(file);
                XmlSerializer xs = new XmlSerializer(typeof(List<T>));
                xs.Serialize(writer, genericList);
                writer.Close();

                if (this.lastModifiedFileName != null)
                {
                    if (!this.lastModifiedFileName.Equals(String.Empty))
                    {
                        IsolatedStorageFileStream lastModifiedFile = isoStorage.OpenFile(this.lastModifiedFileName, FileMode.Create);
                        writer = new StreamWriter(lastModifiedFile);
                        writer.WriteLine(DateTime.Now.ToString());
                        writer.Close();
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
            finally
            {
                if (writer != null)
                    writer.Dispose();
            }
        }

        public Boolean IsStale(TimeSpan shelfLife)
        {
            Boolean result = true;
            if (!this.lastModifiedFileName.Equals(String.Empty))
            {
                TextReader reader = null;
                try
                {
                    IsolatedStorageFile isoStorage = IsolatedStorageFile.GetUserStoreForApplication();
                    IsolatedStorageFileStream file = isoStorage.OpenFile(this.lastModifiedFileName, FileMode.Open);
                    reader = new StreamReader(file);
                    DateTime lastModifiedDate;
                    if (DateTime.TryParse(reader.ReadLine(), out lastModifiedDate))
                    {
                        if ((DateTime.Now - lastModifiedDate) < shelfLife)
                        {
                            result = false;
                        }
                    }
                    reader.Close();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                }
                finally
                {
                    if (reader != null)
                        reader.Dispose();
                }
            }
            return result;
        }
    }
}