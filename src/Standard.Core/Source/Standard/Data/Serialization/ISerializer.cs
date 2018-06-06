using System;
using System.IO;

namespace Standard.Data.Serialization
{
    /// <summary>
    /// Provides functionality for serializing and deserializing objects.
    /// </summary>
    public interface ISerializer
    {
        /// <summary>
        /// Serializes an object, or graph of objects with the given root to the provided stream.
        /// </summary>
        /// <param name="stream">The stream where the formatter puts the serialized data. This stream can reference a variety of backing stores (such as files, network, memory, and so on).</param>
        /// <param name="graph">The object, or root of the object graph, to serialize. All child objects of this root object are automatically serialized.</param>
        void Serialize(Stream stream, object graph);

        /// <summary>
        /// Deserializes the data on the provided stream and reconstitutes the graph of objects.
        /// </summary>
        /// <param name="data">The byte array that contains the data to deserialize.</param>
        object Deserialize(byte[] data);

        /// <summary>
        /// Deserializes the data on the provided stream and reconstitutes the graph of objects.
        /// </summary>
        /// <param name="text">The text that contains the data to deserialize.</param>
        object Deserialize(string text);

        /// <summary>
        /// Deserializes the data on the provided stream and reconstitutes the graph of objects.
        /// </summary>
        /// <param name="stream">The stream that contains the data to deserialize.</param>
        object Deserialize(Stream stream);
    }

    /// <summary>
    /// Provides functionality for serializing and deserializing objects.
    /// </summary>
    public interface ISerializer<T>
    {
        /// <summary>
        /// Serializes an object, or graph of objects with the given root to the provided stream.
        /// </summary>
        /// <param name="stream">The stream where the formatter puts the serialized data. This stream can reference a variety of backing stores (such as files, network, memory, and so on).</param>
        /// <param name="graph">The object, or root of the object graph, to serialize. All child objects of this root object are automatically serialized.</param>
        void Serialize(Stream stream, T graph);

        /// <summary>
        /// Deserializes the data on the provided stream and reconstitutes the graph of objects.
        /// </summary>
        /// <param name="data">The byte array that contains the data to deserialize.</param>
        T Deserialize(byte[] data);

        /// <summary>
        /// Deserializes the data on the provided stream and reconstitutes the graph of objects.
        /// </summary>
        /// <param name="text">The text that contains the data to deserialize.</param>
        T Deserialize(string text);

        /// <summary>
        /// Deserializes the data on the provided stream and reconstitutes the graph of objects.
        /// </summary>
        /// <param name="stream">The stream that contains the data to deserialize.</param>
        T Deserialize(Stream stream);        
    }
}
