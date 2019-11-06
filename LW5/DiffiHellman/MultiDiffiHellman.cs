using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiffiHellman
{
    class MultiDiffiHellman
    {
        /// <summary>
        /// Модуль
        /// </summary>
        public long P;
        /// <summary>
        /// Первообразный корень
        /// </summary>
        public long q;
        /// <summary>
        /// Участники обмена 
        /// </summary>
        public List<Node> Participants; 
        public MultiDiffiHellman() => P = 0;
        /// <summary>
        /// Инициализирует протокол участниками, модулем и новыми ключами
        /// </summary>
        /// <param name="prts">Участники обмена</param>
        /// <param name="p">Модуль</param>
        public MultiDiffiHellman(List<Node> prts, long p, int maxSecret)
        {
            Participants = new List<Node>();
            Participants = prts;
            P = p;
            q = (long)TakePrimitiveRoot(p);
            for (int i = 0; i < Participants.Count; i++)
            {
                Participants[i] = new Node(Participants[i].ID, CreateSecretKey(1, maxSecret), this.q, this.q);
            }
        }
        /// <summary>
        /// Сравнивает закрытые ключи пользователей
        /// </summary>
        /// <returns>Успешность результата</returns>
        public bool CompareCommonKeys()
        {
            long result = 1;
            for (int i = 0; i < Participants.Count; i++)
                result *= Participants[i].SK;           
            long g = (long)PowMod((ulong)q, (ulong)result, (ulong)P);
            foreach (Node i in Participants)
            {
                if(i.CommonKey == g)
                {
                    continue;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// Осуществляет обмен ключами между пользователями
        /// </summary>
        public void ExchangeOfGroup()
        {
            List<string> sets = CreateCombinations();
            Console.WriteLine("Users are changing keys...");
            for (int i = 0; i < sets.Count; i++)
            {
                string[] f = sets[i].Split(' ');
                for (int j = 1; j < f.Length; j++)
                {
                    if (j == 1)
                    {
                        Participants[Convert.ToInt32(f[j - 1])] = new Node(Participants[Convert.ToInt32(f[j - 1])].ID, Participants[Convert.ToInt32(f[j - 1])].SK, Participants[Convert.ToInt32(f[j - 1])].CommonKey, q);
                        Console.Write($"\t{Participants[Convert.ToInt32(f[j - 1])].ID}");
                    }
                    Node d = Participants[Convert.ToInt32(f[j])];
                    Console.Write($" -> {Participants[Convert.ToInt32(f[j])].ID}");
                    SendToParticipant(Participants[Convert.ToInt32(f[j - 1])], ref d);
                    if(j == f.Length - 1)
                    {
                        Participants[Convert.ToInt32(f[j])] = new Node(d.ID, d.SK, (long)PowMod((ulong)d.RK, (ulong)d.SK, (ulong)P), d.RK);
                    }
                    else
                    {
                        Participants[Convert.ToInt32(f[j])] = d;
                    }
                }
                Console.WriteLine();
            }
        }
        /// <summary>
        /// Пересылает значение пользователю
        /// </summary>
        /// <param name="first">Отправитель</param>
        /// <param name="second">Получатель</param>
        public void SendToParticipant(Node first, ref Node second)
        {
            long s_sk = (long)PowMod((ulong)first.RK, (ulong)first.SK, (ulong)this.P);
            second = new Node(second.ID, second.SK, second.CommonKey, s_sk);
        }
        /// <summary>
        /// Создаёт кратчайшие пути для обмена ключами
        /// </summary>
        /// <returns>Пути для получения ключа каждым пользователем</returns>
        public List<string> CreateCombinations()
        {
            string[] pt = Enumerable.Range(0, Participants.Count).Select(x => x.ToString()).ToArray();      
            int m = Participants.Count, n = Participants.Count;
            List<string> paths = new List<string>();
            List<string> paths_new = new List<string>();
            for (int i = 0; i < Math.Pow(m, n); i++)
            {
                string s = "";
                int ii = i;
                for (int j = 0; j < n; j++)
                {
                    s = pt[ii % m] + " " + s;
                    ii /= m;
                }
                for (int k = 0; k < Participants.Count; k++)
                {                   
                    if (s.Contains(k.ToString()) && s.IndexOf(k.ToString()) == s.LastIndexOf(k.ToString()))
                    {
                        if (k == Participants.Count - 1)
                        {
                            paths.Add(s);
                        }
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            for (int j = Participants.Count-1; j >=0 ; j--)
            {
                for (int i = 0; i < paths.Count; i++)
                {
                    if (paths[i].EndsWith((j+" ").ToString()))
                    {
                        paths_new.Add(paths[i].Substring(0,paths[i].Length-1));
                        break;
                    }
                }
            }
            return paths_new;
        }
        /// <summary>
        /// Создаёт число в заданном диапазоне
        /// </summary>
        /// <param name="min">Минимальное значение</param>
        /// <param name="max">Максимальное значение</param>
        /// <returns>Число</returns>
        long CreateSecretKey(int min, int max)
        {
            Random rand_0 = new Random((int)DateTime.Now.Ticks),
                rand_1 = new Random();
            return ((rand_0.Next(min, max)* rand_1.Next(min, max/2))%max)+1;
        }
        /// <summary>
        /// Алгоритм быстрого возведения в степень по модулю
        /// </summary>
        /// <param name="number">Число</param>
        /// <param name="pow">Степень</param>
        /// <param name="module">Модуль</param>
        /// <returns>Значение по модулю</returns>
        public static ulong PowMod(ulong number, ulong pow, ulong module)
        {
            string q = Convert.ToString((long)pow, 2); //Двоичное представление степени
            decimal s = 1, c = number; //Инициализация
            for (int i = q.Length - 1; i >= 0; i--)
            {
                if (q[i] == '1')
                {
                    s = (s * c) % module;
                }
                c = (c * c) % module;
            }
            return (ulong)s;
        }
        /// <summary>
        /// Производит поиск генератора всей группы
        /// </summary>
        /// <param name="primeNum">Порядок группы</param>
        /// <returns>Генератор</returns>
        protected decimal TakePrimitiveRoot(decimal primeNum)
        {
            for (ulong i = 0; i < primeNum; i++)
                if (IsPrimitiveRoot(primeNum, i))
                    return i;
            return 0;
        }
        /// <summary>
        /// Проверка на примитивность
        /// </summary>
        /// <param name="p">Порядок</param>
        /// <param name="a">Элемент</param>
        /// <returns></returns>
        public bool IsPrimitiveRoot(decimal p, decimal a)
        {
            if (a == 0 || a == 1)
                return false;
            decimal last = 1;
            HashSet<decimal> set = new HashSet<decimal>();
            for (ulong i = 0; i < p - 1; i++)
            {
                last = (last * a) % p;
                if (set.Contains(last)) // Если повтор
                    return false;
                set.Add(last);
            }
            return true;
        }
    }
    public struct Node
    {
        string _id;
        long _sk;
        long _ck;
        long _rk; //received
        public string ID
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }
        public long SK
        {
            get
            {
                return _sk;
            }
            set
            {
                _sk = value;
            }
        }
        public long RK
        {
            get
            {
                return _rk;
            }
            set
            {
                _rk = value;
            }
        }
        public long CommonKey
        {
            get
            {
                return _ck;
            }
            set
            {
                _ck = value;
            }
        }
        public Node(string name)
        {
            this._sk = 0;
            this._id = name;
            this._ck = 0;
            this._rk = 0;
        }
        public Node(string name, long sk, long ck, long rk)
        {
            this._sk = sk;
            this._id = name;
            this._rk = (long)rk;
            this._ck = ck;
            SK = sk;
            ID = name;
            RK = (long)rk;
            CommonKey = _ck;
        }
    }
}
