//======================================================================
// XNA Terrain Editor
// Copyright (C) 2008 Eric Grossinger
// http://psycad007.spaces.live.com/
//======================================================================
using System.Xml;
using System.IO;
using System;

public class Serializer
{

    public object Load(string path, Type type)
    {

        System.IO.FileStream fileStream = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read);

        return this.Load(fileStream, type);

    }

    public object Load(System.IO.Stream stream, Type type)
    {

        System.IO.TextReader textReader;
        object m_object;

        try
        {

            //create an TextReader using a FileStream
            textReader = new System.IO.StreamReader(stream);

            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(type);

            //deserialize using the TextReader
            m_object = serializer.Deserialize(textReader);
        }

        catch (Exception ex)
        {
            throw ex;
        }

        //close the reader
        if (textReader != null)
            textReader.Close();

        return m_object;

    }

    public void Save(string path, object o)
    {

        System.IO.FileStream fileStream = new System.IO.FileStream(path, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.Write);

        this.Save(fileStream, o);

    }

    public void Save(System.IO.Stream stream, object o)
    {
        System.IO.TextWriter textWriter;
        Type type = o.GetType();

        try
        {
            // create an TextWriter using a FileStream
            textWriter = new System.IO.StreamWriter(stream, System.Text.Encoding.UTF8);
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(type);
            serializer.Serialize(textWriter, o);
        }
        catch (Exception ex)
        {
            throw ex;
        }

        // close the writer
        if (textWriter != null)
            textWriter.Close();
    }

}