using System;
using System.Collections;

namespace PSZI_lr1_v2
{
    public class HashFunctions
    {

        // Hi = E_M_i (H_(i-1)) xor (H_(i-1))
        public static BitArray schema1(BitArray[] textBlocks, BitArray H0)
        {
            BitArray Mi, Hi = new BitArray(H0);
            BitArray chipher = null;
            for (int i = 0; i < textBlocks.Length; i++)
            {
                Mi = new BitArray(textBlocks[i]);
                GeneratorDESKey generatorKey = new GeneratorDESKey(Mi);

                CBC cbc = new CBC(generatorKey, Hi); 

                chipher = cbc.Encrypte(Hi, chipher);

                Hi.Xor(chipher);
            }
            return Hi;
        }

        public static BitArray schema2(BitArray[] textBlocks, BitArray H0)
        {

            BitArray Mi, Hi = new BitArray(H0), chipher = new BitArray(H0);
            for (int i = 0; i < textBlocks.Length; i++)
            {
                Mi = new BitArray(textBlocks[i]);
                GeneratorDESKey generatorKey = new GeneratorDESKey(Hi);

                CBC cbc = new CBC(generatorKey, Hi); // НЕЛЬЗЯ ЛИ ПРОСТО ЗАДАТЬ КЛЮЧ КАК textBlocks ??? без генератора то есть??

                chipher = cbc.Encrypte(Mi, chipher);

                BitArray temp = new BitArray(chipher);
                temp.Xor(Hi);
                temp.Xor(Mi);
                Hi = new BitArray(temp);
            }
            return Hi;
        }

        public static BitArray schema3(BitArray[] textBlocks, BitArray H0)
        {

            BitArray Mi, Hi = new BitArray(H0), chipher = new BitArray(H0);
            for (int i = 0; i < textBlocks.Length; i++)
            {
                Mi = new BitArray(textBlocks[i]);
                BitArray P = new BitArray(Mi);
                P.Xor(Hi);

                GeneratorDESKey generatorKey = new GeneratorDESKey(Hi);

                CBC cbc = new CBC(generatorKey, Hi); // НЕЛЬЗЯ ЛИ ПРОСТО ЗАДАТЬ КЛЮЧ КАК textBlocks ??? без генератора то есть??

                chipher = cbc.Encrypte(P, P);
                BitArray temp = new BitArray(chipher);

                temp.Xor(Hi);
                temp.Xor(Mi);
                Hi = new BitArray(temp);
            }
            return Hi;
        }

        public static BitArray schema4(BitArray[] textBlocks, BitArray H0)
        {

            BitArray Mi, Hi = new BitArray(H0), chipher = new BitArray(H0);
            for (int i = 0; i < textBlocks.Length; i++)
            {
                Mi = new BitArray(textBlocks[i]);
                BitArray P = new BitArray(Mi);
                P.Xor(Hi);

                GeneratorDESKey generatorKey = new GeneratorDESKey(Hi);

                CBC cbc = new CBC(generatorKey, Hi); // НЕЛЬЗЯ ЛИ ПРОСТО ЗАДАТЬ КЛЮЧ КАК textBlocks ??? без генератора то есть??

                chipher = cbc.Encrypte(P, P);
                BitArray temp = new BitArray(chipher);

                temp.Xor(Mi);
                Hi = new BitArray(temp);
            }
            return Hi;
        }

        public static BitArray PBGV(BitArray[] textBlocks, BitArray G0, BitArray H0)
        {
            BitArray L, R, G = new BitArray(G0), H = new BitArray(H0), chipherG = new BitArray(H0), chipherH = new BitArray(H0);
            for(int i = 0; i < textBlocks.Length - 1; i++)
            {
                L = new BitArray(textBlocks[i]);
                R = new BitArray(textBlocks[i + 1]);

                BitArray keyG = new BitArray(L);
                keyG.Xor(H);
                GeneratorDESKey generatorKeyG = new GeneratorDESKey(keyG);

                CBC cbcG = new CBC(generatorKeyG, G);

                BitArray inputG = new BitArray(R);
                inputG.Xor(G);
                chipherG = cbcG.Encrypte(inputG, chipherG);

                BitArray tempG = new BitArray(chipherG);
                tempG.Xor(R).Xor(G).Xor(H);

                BitArray keyH = new BitArray(L);
                keyH.Xor(R);
                GeneratorDESKey generatorKeyH = new GeneratorDESKey(keyH);

                CBC cbcH = new CBC(generatorKeyG, H);
                BitArray inputH = new BitArray(H);
                inputH.Xor(G);
                chipherH = cbcG.Encrypte(inputH, chipherH);

                BitArray tempH = new BitArray(chipherH);
                tempH.Xor(L).Xor(G).Xor(H);

                H = tempH;
                G = tempG;
            }
            return G.Append(H);
        }

        public static BitArray QG(BitArray[] textBlocks, BitArray G0, BitArray H0)
        {
            BitArray L, R, G = new BitArray(G0), H = new BitArray(H0), chipherW = new BitArray(H0), chipherG = new BitArray(H0);
            for (int i = 0; i < textBlocks.Length - 1; i++)
            {
                L = new BitArray(textBlocks[i]);
                R = new BitArray(textBlocks[i + 1]);

                GeneratorDESKey generatorKeyW = new GeneratorDESKey(L);
                CBC cbcW = new CBC(generatorKeyW, G);
                BitArray inputW = new BitArray(G);
                inputW.Xor(R);
                BitArray W = cbcW.Encrypte(inputW, chipherW);
                W.Xor(R).Xor(H);

                GeneratorDESKey generatorKeyG = new GeneratorDESKey(R);
                CBC cbcG = new CBC(generatorKeyW, G);
                BitArray inputG = new BitArray(W);
                inputG.Xor(L);
                BitArray tempG = cbcW.Encrypte(inputG, chipherG);
                tempG.Xor(G).Xor(H).Xor(L);

                H = W.Xor(G);
                G = tempG;
            }
            return G.Append(H);
        }
    }
}
