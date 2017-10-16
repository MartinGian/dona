using System.Text;

namespace dona.Forms.Common
{
    public static class StringExtensions
    {
        public static string ReplaceAccentsInVocals(this string x)
        {
            var s = new StringBuilder(x);

            s.Replace("á", "a");
            s.Replace("Á", "A");

            s.Replace("é", "e");
            s.Replace("É", "E");

            s.Replace("í", "i");
            s.Replace("Í", "I");

            s.Replace("ó", "o");
            s.Replace("Ó", "O");

            s.Replace("ú", "u");
            s.Replace("Ú", "U");

            return s.ToString();
        }
    }
}
