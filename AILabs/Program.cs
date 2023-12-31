using AILabs.LabAnts;
using AILabs.HammingNetwork;
using AILabs.Swarm;
using AILabs.SimulatedAnnealing;
using AILabs.Genetic;
using AILabs.HebbNetwork;
using AILabs.MachineLearning;
using AILabs.FuzzyLogic;

namespace AILabs
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            // ����������
            //Application.Run(new Form1());

            // �������
            //Application.Run(new Form2());

            // ��� ������
            //Application.Run(new Form3());

            // �������� ������
            //Application.Run(new Form4());

            // ������������
            //Application.Run(new Form5());

            // ����
            //Application.Run(new Form6());

            // �������� ��������
            //Application.Run(new Form7());

            // ��ר���� ������
            Application.Run(new FormFL());
        }
    }
}