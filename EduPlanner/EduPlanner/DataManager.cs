﻿using System;
using System.Windows.Threading;
using System.IO;
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace EduPlanner {

    public static class DataManager {

        public static string SAVEFILEPATH = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\EduPlanner";
        public const string APPLICATIONNAME = "EduPlanner";
        public const int DAYCOUNT = 7;

        public static Schedule schedule;

        public static MainWindow mainWindow;

        public static Settings settings;

    }

    public class Data {
        DispatcherTimer timer = new DispatcherTimer();

        public int saveTime = 1000;

        private string appdataPath = DataManager.SAVEFILEPATH + @"\Appdata.bin";
        private string settingsPath = DataManager.SAVEFILEPATH + @"\Settings.bin";

        public Data() { }

        public void Save() {
            if (!Directory.Exists(DataManager.SAVEFILEPATH))
                Directory.CreateDirectory(DataManager.SAVEFILEPATH);

            WriteToBinaryFile(appdataPath, DataManager.schedule, false);
            WriteToBinaryFile(settingsPath, DataManager.settings, false);

            //WriteToJsonFile(appdataPath, DataManager.schedule, false);
            //WriteToJsonFile(settingsPath, DataManager.settings, false);

            //WriteToXmlFile(appdataPath, DataManager.schedule, false);
            //WriteToXmlFile(settingsPath, DataManager.settings, false);
        }

        public void Load() {
            if (DataManager.schedule == null)
                DataManager.schedule = new Schedule();
            if (DataManager.settings == null)
                DataManager.settings = new Settings();

            if (File.Exists(appdataPath))
                DataManager.settings = ReadFromBinaryFile<Settings>(settingsPath);

            if (File.Exists(settingsPath))
                DataManager.schedule = ReadFromBinaryFile<Schedule>(appdataPath);
        }

        /// <summary>
        /// Writes the given object instance to a Json file.
        /// <para>Object type must have a parameterless constructor.</para>
        /// <para>Only Public properties and variables will be written to the file. These can be any type though, even other classes.</para>
        /// <para>If there are public properties/variables that you do not want written to the file, decorate them with the [JsonIgnore] attribute.</para>
        /// </summary>
        /// <typeparam name="T">The type of object being written to the file.</typeparam>
        /// <param name="filePath">The file path to write the object instance to.</param>
        /// <param name="objectToWrite">The object instance to write to the file.</param>
        /// <param name="append">If false the file will be overwritten if it already exists. If true the contents will be appended to the file.</param>
        public static void WriteToJsonFile<T>(string filePath, T objectToWrite, bool append = false) where T : new() {
            TextWriter writer = null;
            try {
                var contentsToWriteToFile = JsonConvert.SerializeObject(objectToWrite);
                writer = new StreamWriter(filePath, append);
                writer.Write(contentsToWriteToFile);
            } finally {
                if (writer != null)
                    writer.Close();
            }
        }

        /// <summary>
        /// Reads an object instance from an Json file.
        /// <para>Object type must have a parameterless constructor.</para>
        /// </summary>
        /// <typeparam name="T">The type of object to read from the file.</typeparam>
        /// <param name="filePath">The file path to read the object instance from.</param>
        /// <returns>Returns a new instance of the object read from the Json file.</returns>
        public static T ReadFromJsonFile<T>(string filePath) where T : new() {
            TextReader reader = null;
            try {
                reader = new StreamReader(filePath);
                var fileContents = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<T>(fileContents);
            } finally {
                if (reader != null)
                    reader.Close();
            }
        }

        /// <summary>
        /// Writes the given object instance to a binary file.
        /// <para>Object type (and all child types) must be decorated with the [Serializable] attribute.</para>
        /// <para>To prevent a variable from being serialized, decorate it with the [NonSerialized] attribute; cannot be applied to properties.</para>
        /// </summary>
        /// <typeparam name="T">The type of object being written to the XML file.</typeparam>
        /// <param name="filePath">The file path to write the object instance to.</param>
        /// <param name="objectToWrite">The object instance to write to the XML file.</param>
        /// <param name="append">If false the file will be overwritten if it already exists. If true the contents will be appended to the file.</param>
        public static void WriteToBinaryFile<T>(string filePath, T objectToWrite, bool append = false) {
            using (Stream stream = System.IO.File.Open(filePath, append ? FileMode.Append : FileMode.Create)) {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                binaryFormatter.Serialize(stream, objectToWrite);
            }
        }

        /// <summary>
        /// Reads an object instance from a binary file.
        /// </summary>
        /// <typeparam name="T">The type of object to read from the XML.</typeparam>
        /// <param name="filePath">The file path to read the object instance from.</param>
        /// <returns>Returns a new instance of the object read from the binary file.</returns>
        public static T ReadFromBinaryFile<T>(string filePath) {
            using (Stream stream = System.IO.File.Open(filePath, FileMode.Open)) {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                return (T)binaryFormatter.Deserialize(stream);
            }
        }

        /// <summary>
        /// Writes the given object instance to an XML file.
        /// <para>Only Public properties and variables will be written to the file. These can be any type though, even other classes.</para>
        /// <para>If there are public properties/variables that you do not want written to the file, decorate them with the [XmlIgnore] attribute.</para>
        /// <para>Object type must have a parameterless constructor.</para>
        /// </summary>
        /// <typeparam name="T">The type of object being written to the file.</typeparam>
        /// <param name="filePath">The file path to write the object instance to.</param>
        /// <param name="objectToWrite">The object instance to write to the file.</param>
        /// <param name="append">If false the file will be overwritten if it already exists. If true the contents will be appended to the file.</param>
        public static void WriteToXmlFile<T>(string filePath, T objectToWrite, bool append = false) where T : new() {
            TextWriter writer = null;
            try {
                var serializer = new XmlSerializer(typeof(T));
                writer = new StreamWriter(filePath, append);
                serializer.Serialize(writer, objectToWrite);
            } finally {
                if (writer != null)
                    writer.Close();
            }
        }

        /// <summary>
        /// Reads an object instance from an XML file.
        /// <para>Object type must have a parameterless constructor.</para>
        /// </summary>
        /// <typeparam name="T">The type of object to read from the file.</typeparam>
        /// <param name="filePath">The file path to read the object instance from.</param>
        /// <returns>Returns a new instance of the object read from the XML file.</returns>
        public static T ReadFromXmlFile<T>(string filePath) where T : new() {
            TextReader reader = null;
            try {
                var serializer = new XmlSerializer(typeof(T));
                reader = new StreamReader(filePath);
                return (T)serializer.Deserialize(reader);
            } finally {
                if (reader != null)
                    reader.Close();
            }
        }
    }
}
