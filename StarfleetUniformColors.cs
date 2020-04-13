using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace DesignPatterns.GangOfFour.Creational.Prototype
{
    /// <summary>
    /// Prototype Design Pattern.
    /// 
    /// Definition: The Prototype Design Pattern specifies the kind of objects to create using
    ///             a prototypical instance and creates new objects by copying this prototype.
    /// 
    /// This pattern hides the creation of objects from clients 
    /// (similar to the other Creational Gang Of Four Patterns---Abstract Factory, Builder, and Factory Method).
    /// 
    /// The Prototype pattern differs from the other Creational Patterns in that it returns a new object
    /// that is initialized with values that are copied from a sample object (a prototype).  

    /// The other creational patterns create/return non-initialized, "dummy" objects.
    /// </summary>
    public class Program
    {

        /// <summary>
        /// This program implements the Protoype Pattern by creating different, Star Trek Starlfeet Uniforms
        /// 
        /// 1)  The prototype in this example is the built-in, .NET ICloneable interface.
        /// 
        /// 2)  The ConcretePrototype, StarfleetUniformColor, overrides the Clone() method
        ///     by returning Deep or Shallow Copies (Clones) of the StarfleetUniformColor object
        ///     
        /// 3)  The Client, StarfleetUniformColorManager, creates new StarfleetUniformColor objects
        ///     by asking the ConcretePrototype, StarfleetUniformColor, to clone itself
        /// </summary>

        static void Main()
        {
            // StarfleetUniformColorManager is the client in the Prototype Pattern
            var StarfleetUniformColorManager = new StarfleetUniformColorManager();

            // Initialize with standard colors
            StarfleetUniformColorManager[StarfleetUniformColorType.Red] = new StarfleetUniformColor { Red = 255, Blue = 0, Green = 0 };
            StarfleetUniformColorManager[StarfleetUniformColorType.Green] = new StarfleetUniformColor { Red = 0, Blue = 0, Green = 255 };
            StarfleetUniformColorManager[StarfleetUniformColorType.Blue] = new StarfleetUniformColor { Red = 0, Blue = 255, Green = 00 };

            // Here, we add colors for more specific Starfleet Uniform Classes
            StarfleetUniformColorManager[StarfleetUniformColorType.Command] = new StarfleetUniformColor { Red = 255, Blue = 54, Green = 0 };
            StarfleetUniformColorManager[StarfleetUniformColorType.Engineering] = new StarfleetUniformColor { Red = 128, Blue = 211, Green = 128 };
            StarfleetUniformColorManager[StarfleetUniformColorType.Medical] = new StarfleetUniformColor { Red = 211, Blue = 34, Green = 20 };

            // Create "Shallow copies"...
            var basicColor1 = StarfleetUniformColorManager[StarfleetUniformColorType.Red].Clone() as StarfleetUniformColor;
            var basicColor2 = StarfleetUniformColorManager[StarfleetUniformColorType.Engineering].Clone() as StarfleetUniformColor;

            // Creates a "deep copy"...
            var starfleetMeidcalUni = StarfleetUniformColorManager[StarfleetUniformColorType.Medical].Clone(false) as StarfleetUniformColor;

            // User input terminates the program...
            Console.ReadKey();
        }
    }

    /// <summary>
    /// The 'ConcretePrototype' class
    /// </summary>
    [Serializable]

    /// Why is this class Serializable?  
    /// Classes that implement the ICloneable interface must be serializable;
    /// however, if the class has 'event' members, then the 'event' members in the class must use the 'NonSerializable' attribute.
    /// 
    /// Reflection could be used instead of serialization, and it is advisable to perfomance test classes to see which choice performs better.
    /// 
    /// We implement the ICloneable interface to enable cloning of pre-existing objects.  ICloneable has a Clone method that returns
    /// an object that is a clone of the original object.
    /// 
    /// There are two types of clone operations:
    /// 1) Deep Copy:       copies the prototype and all the object's references (this can be very resource intensive)
    /// 2) Shallow Copy:    copies only properties of the object but no object references (less resource intensive and easier to implement)
    /// 
    /// Shallow Copy is easier to implement, because the Object base class already contains a "MemberwiseClone" method which returns a shallow
    /// copy of the object being cloned.
    /// 
    /// Deep Copy is more complicated, because some objects (db connections and Threads, for example) are difficult to copy and in some cases
    /// shouldn't be copied at all.  Care also has to be taken to avoid circular references.

    class StarfleetUniformColor : ICloneable
    {
        // Gets or sets red value
        // Why use bytes instead of ints? RGB values range between 0-255, so we can use the smaller byte type instead.
        public byte Red { get; set; }

        // Gets or sets green value
        public byte Green { get; set; }

        // Gets or sets blue channel
        public byte Blue { get; set; }

        // Returns shallow or deep copy
        public object Clone(bool shallow)
        {
            return shallow ? Clone() : DeepCopy();
        }

        // Creates a shallow copy
        public object Clone()
        {
            Console.WriteLine(
                "Shallow copy of Starfleet Uniform Color RGB: {0,3},{1,3},{2,3}",
                Red, Blue, Green);

            return this.MemberwiseClone();
        }

        // Creates a deep copy
        // Notice how much more complicated the deep copy is to implement...
        public object DeepCopy()
        {
            var stream = new MemoryStream();
            var formatter = new BinaryFormatter();

            //Serialize the StarfleetUniformColor object...
            formatter.Serialize(stream, this);
            stream.Seek(0, SeekOrigin.Begin);

            //Create Deep Copy by Deserializing the stream containing the StarfleetUniformColor object...
            object copy = formatter.Deserialize(stream);
            stream.Close();

            Console.WriteLine(
                "*Deep*  copy of Starfleet Uniform Color RGB: {0,3},{1,3},{2,3}",
                (copy as StarfleetUniformColor).Red,
                (copy as StarfleetUniformColor).Blue,
                (copy as StarfleetUniformColor).Green);

            return copy;
        }
    }

    /// <summary>
    /// Type-safe prototype manager
    /// This is the client in the Prototype Design Pattern.
    /// </summary>
    class StarfleetUniformColorManager
    {
        Dictionary<StarfleetUniformColorType, StarfleetUniformColor> colors = new Dictionary<StarfleetUniformColorType, StarfleetUniformColor>();

        // Gets or sets color
        public StarfleetUniformColor this[StarfleetUniformColorType type]
        {
            get { return colors[type]; }
            set { colors.Add(type, value); }
        }
    }

    /// <summary>
    /// StarfleetUniformColor type enumerations
    /// </summary>
    enum StarfleetUniformColorType
    {
        Red,
        Green,
        Blue,

        Command,
        Engineering,
        Medical
    }
}
