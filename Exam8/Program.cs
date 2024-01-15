
namespace Exam8
{
    internal class Program
    {
        const string SettingsFileName = @"E:\Oix\Projects\Vent\BinaryFile.bin";
        static void Main(string[] args)
        {
            ReadValues();

            Console.ReadKey();
        }

        private static void ReadValues()
        {
            float FloatValue;
            string StringValue;
            int IntValue;
            bool BooleanValue;

            if (!File.Exists(SettingsFileName)) return;
            try
            {
                using (BinaryReader reader = new BinaryReader(File.Open(SettingsFileName, FileMode.Open)))
                {
                    StringValue = reader.ReadString();
                }
                Console.WriteLine(StringValue);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
