using System;
using System.Collections.Generic;
using System.Linq;

namespace Polysat.Solver
{
    internal class Kernel
    {
        private readonly int n;
        /// <summary>
        /// хранилище векторов назначений и масок
        /// </summary>
        private readonly ulong[] data;
        /// <summary>
        /// информация об удаленных векторах
        /// </summary>
        private readonly byte[] removed;
        /// <summary>
        /// размер вектора в байтах
        /// </summary>
        private readonly int vectorSize;
        /// <summary>
        /// смещение в хранилище, привязанное к пространственному индексу сочетания
        /// </summary>
        private readonly IDictionary<Combination, int> offset;

        public Kernel(VectorStore store)
        {
            n = store.n;
            vectorSize = (store.n - 1 - (store.n - 1) % 64) / 64 + 1;
            // 8 векторов в сочетании, данные и маска
            data = new ulong[store.functions.Count * 8 * vectorSize * 2];
            // один бит на вектор в каждом байте сочетания
            removed = store.functions.Values.ToArray();
            offset = new Dictionary<Combination, int>();
            int vIndex = 0;
            foreach (var (c, f) in store.functions)
            {
                offset.Add(c, vIndex++);
            }
            InitializeVectors();
        }
        /// <summary>
        /// Инициализация векторов сочетаний
        /// </summary>
        private void InitializeVectors()
        {
            foreach (var c in GetCombinations())
            {
                var vectors = GetVectors(c);
                foreach (var v in vectors)
                {
                    v.SetBit(c.Index.x0, (v.vIndex >> 2) & 1);
                    v.SetBit(c.Index.x1, (v.vIndex >> 1) & 1);
                    v.SetBit(c.Index.x2, v.vIndex & 1);
                }
            }
        }
        /// <summary>
        /// Список доступных сочетаний
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Combination> GetCombinations() => offset.Keys;
        /// <summary>
        /// Список доступных векторов сочетания
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public IEnumerable<Vector> GetVectors(Combination c)
        {
            for (byte i = 0; i < 8; i++)
            {
                if (!Removed(c, i))
                {
                    yield return new Vector(n, c, i,
                        new ArraySegment<ulong>(data, (offset[c] * 16 + i) * vectorSize, vectorSize),
                        new ArraySegment<ulong>(data, (offset[c] * 16 + 8 + i) * vectorSize, vectorSize));
                }
            }
        }
        /// <summary>
        /// удаление всех векторов в сочетании кроме указанного
        /// </summary>
        /// <param name="v"></param>
        public void RemoveExcept(Vector v)
        {
            removed[offset[v.c]] = (byte)(0xff ^ (1 << v.vIndex));
        }
        /// <summary>
        /// Удаление вектора
        /// </summary>
        /// <param name="v"></param>
        public void Remove(Vector v)
        {
            removed[offset[v.c]] |= (byte)(1 << v.vIndex);
        }
        /// <summary>
        /// Групповое удаление векторов назначений сочетания
        /// </summary>
        /// <param name="c">сочетание</param>
        /// <param name="rn">маска удаляемых векторов</param>
        /// <returns></returns>
        public bool Remove(Combination c, byte rn)
        {
            var ro = removed[offset[c]];
            removed[offset[c]] |= rn;
            return removed[offset[c]] != ro;
        }

        /// <summary>
        /// Проверка существования вектора по сочетанию и индексу
        /// </summary>
        /// <param name="comb"></param>
        /// <param name="vIndex"></param>
        /// <returns></returns>
        public bool Removed(Combination comb, byte vIndex)
        {
            return (removed[offset[comb]] & (1 << vIndex)) > 0;
        }
        /// <summary>
        /// Проверка на существование вектора
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public bool Removed(Vector v)
        {
            return (removed[offset[v.c]] & (1 << v.vIndex)) > 0;
        }

        /// <summary>
        /// Сохранение состояния ядра
        /// </summary>
        /// <returns></returns>
        public KernelSnapshot Snapshot()
        {
            return new KernelSnapshot(data, removed);
        }
        /// <summary>
        /// Восстановление состояния ядра
        /// </summary>
        /// <param name="snapshot"></param>
        public void Restore(KernelSnapshot snapshot)
        {
            snapshot.data.AsSpan().CopyTo(data.AsSpan());
            snapshot.removed.AsSpan().CopyTo(removed.AsSpan());
        }
    }
}
