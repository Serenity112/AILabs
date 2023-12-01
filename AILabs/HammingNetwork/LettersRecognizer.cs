using MathLib;

namespace AILabs.HammingNetwork
{
    public class LettersRecognizer
    {
        private string _resourcesPath = @"..\..\..\HammingNetwork\Resources\";

        private string[] _lettersFileNames = new string[]
        {
            "С.png",
            "А.png",
            "Н.png",
            "Ч.png",
            "О.png",
            "К.png",
        };

        private HammingNetwork _hammingNetwork;

        public LettersRecognizer()
        {
            List<NumericVector> _inputVectrors = new List<NumericVector>();
            foreach (string file in _lettersFileNames)
            {
                Bitmap bitmap = new Bitmap($"{_resourcesPath}{file}");
                _inputVectrors.Add(ImageUtils.ConvertImageToBinaryVector(bitmap));
            }

            _hammingNetwork = new HammingNetwork(_inputVectrors);
        }

        public int Recognize(Bitmap bitmap)
        {
            NumericVector vect = ImageUtils.ConvertImageToBinaryVector(bitmap);
            int group = _hammingNetwork.GetVectorGroup(vect);
            return group;
        }

        public Bitmap GetOriginalImage(int group)
        {
            return new Bitmap($"{_resourcesPath}{_lettersFileNames[group]}");
        }
    }
}
