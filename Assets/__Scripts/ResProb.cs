
namespace ProspectorPrototyp
{
    public class ResProb
    {
        public int cAll = 0;
        public int cWin = 0;
        public int cLoss = 0;
        public int cDraw = 0;

        public  double ProbeWin()
        { 
            if (cAll == 0) return 0;

            return cWin / cAll;
        }

        public double ProbeLoss()
        {
            if (cLoss == 0) return 0;

            return cLoss / cAll;
        }

        public double FinaleProbCombination() 
        {
            return System.Math.Pow((double)(cWin + cDraw / 2) / cAll, 3);
        }

    }
}
