using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace Assets
{

    public class ReseauNeural
    {
        float[] Entree;
        float[] Sortie;

        Couche[] LesCoucheNeural;

        Neurone[] NeuroneSortie;
        int NbNeuronneMax;

        public ReseauNeural()
        {

        }

        public string SaveToString()
        {
            string result = "";
            for(int i=0;i<LesCoucheNeural.Length; i++)
            {
                if (i != 0) result += ":";
                result += LesCoucheNeural[i].SaveToString();
            }
            result += "!";
            for(int i=0;i<NeuroneSortie.Length;i++)
            {
                if (i != 0) result += ";";
                result += NeuroneSortie[i].SaveToString();
            }
            return result;
        }

        public void LoadByString(string SaveString)
        {
            string[] part = SaveString.Split('!');

            string[] couches = part[0].Split(':');
            for(int i=0;i<couches.Length;i++)
            {
                LesCoucheNeural[i].LoadByString(couches[i]);
            }

            string[] Neuro = part[1].Split(';');
            for (int i = 0; i < Neuro.Length; i++)
            {
                NeuroneSortie[i].LoadByString(Neuro[i]);
            }
        }

        public static ReseauNeural CreateRandom(int nbEntree, int nbSortie, int nbCouche, float RandomIntensity)
        {
            ReseauNeural output = new ReseauNeural(nbEntree, nbSortie, nbCouche);
            output.Randomize(RandomIntensity);
            return output;
        }


        public ReseauNeural(int nbEntree, int nbSortie, int nbCouche)
        {
            Entree = new float[nbEntree];
            Sortie = new float[nbSortie];
            NeuroneSortie = new Neurone[nbSortie];
            for (int i = 0; i < nbSortie; i++)
            {
                NeuroneSortie[i] = new Neurone(nbEntree);
            }
            if (nbCouche < 0) nbCouche = 1;
            NbNeuronneMax = nbEntree > nbSortie ? nbEntree : nbSortie;

            LesCoucheNeural = new Couche[nbCouche];
            for (int i = 0; i < nbCouche; i++)
            {
                if (i == 0)
                    LesCoucheNeural[i] = new Couche(nbEntree, NbNeuronneMax);
                else
                    LesCoucheNeural[i] = new Couche(NbNeuronneMax, NbNeuronneMax);
            }
        }

        public void Randomize(float intensity)
        {
            for (int i = 0; i < LesCoucheNeural.Length; i++)
            {
                LesCoucheNeural[i].Randomize(intensity);
            }
            for (int i = 0; i < NeuroneSortie.Length; i++)
            {
                NeuroneSortie[i].Randomize(intensity);
            }
        }


        public float[] Calcul(float[] _Entree)
        {
            Entree = _Entree;
            float[] valeurIntermediaire = new float[NbNeuronneMax];
            for (int i = 0; i < LesCoucheNeural.Length; i++)
            {
                if (i == 0)
                {
                    valeurIntermediaire = LesCoucheNeural[0].Calcul(Entree);

                }
                else
                {
                    valeurIntermediaire = LesCoucheNeural[i].Calcul(valeurIntermediaire);
                }
            }
            for (int i = 0; i < Sortie.Length; i++)
            {
                Sortie[i] = NeuroneSortie[i].Calcul(valeurIntermediaire);
            }
            return Sortie;

        }

        

        public class Neurone
        {

            
            private float[] Entree;
            private float[] Coefficient;

            public float sortie { get; private set; }

            #region Sauvegarde
            public string SaveToString()
            {
                string result = "";
                for (int i = 0; i < Coefficient.Length; i++)
                {
                    if (i != 0) result += "_";
                    result += Coefficient[i].ToString();

                }
                return result;
            }

            public void LoadByString(string SaveString)
            {
                string[] coef = SaveString.Split('_');
                
                for (int i = 0; i < coef.Length; i++)
                {
                    Coefficient[i] = Convert.ToSingle(coef[i]);
                }
            }
            #endregion

            public void Randomize(float intensity)
            {
                for (int i = 0; i < Coefficient.Length; i++)
                {
                    Coefficient[i] += UnityEngine.Random.Range(-intensity, intensity);
                }
            }

            public Neurone(int nbEntrees)
            {
                Entree = Coefficient = new float[nbEntrees];
            }


            public void SetInputs(float[] _Input)
            {
                Entree = _Input;
            }

            public float Calcul(float[] Entree)
            {
                float Somme = 0;
                for (int i = 0; i < Entree.Length; i++)
                {
                    Somme += Entree[i] * Coefficient[i];

                }
                return sortie = Somme / Entree.Length;
            }

        }
        public class Couche
        {
            private Neurone[] LesNeurones;
            private float[] Entrees;
            public float[] Sorties { get; private set; }

            public string SaveToString()
            {
                string result = "";
                for(int i=0;i<LesNeurones.Length;i++)
                {
                    if (i != 0) result += ";";
                    result += LesNeurones[i].SaveToString();
                }
                return result;
            }

            public void LoadByString(string saveString)
            {
                string[] Neuro = saveString.Split(';');
                for(int i = 0;i<Neuro.Length;i++)
                {
                    LesNeurones[i].LoadByString(Neuro[i]);
                }

            }

            public void Randomize(float intensity)
            {
                for (int i = 0; i < LesNeurones.Length; i++)
                    LesNeurones[i].Randomize(intensity);
            }

            public float[] Calcul(float[] _Entrees)
            {
                Entrees = _Entrees;
                Sorties = new float[LesNeurones.Length];
                for (int i = 0; i < LesNeurones.Length; i++)
                {
                    Sorties[i] = LesNeurones[i].Calcul(Entrees);
                }
                return Sorties;
            }

            public Couche(int nbEntrees, int nbSorties)
            {
                LesNeurones = new Neurone[nbSorties];
                for (int i = 0; i < nbSorties; i++)
                {
                    LesNeurones[i] = new Neurone(nbEntrees);
                }

            }

        }
    }




}
