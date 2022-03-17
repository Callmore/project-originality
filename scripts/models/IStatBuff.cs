using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectOriginality.Models
{
    public interface IStatBuff
    {
        double Bonus { get; }
    }

    public struct BuffFlat : IStatBuff
    {
        public double Bonus { get; }

        public BuffFlat(double amount)
        {
            Bonus = amount;
        }
    }

    public struct BuffAdditive : IStatBuff
    {
        public double Bonus { get; }

        public BuffAdditive(double amount)
        {
            Bonus = amount;
        }
    }

    public struct BuffMultipicitive : IStatBuff
    {
        public double Bonus { get; }

        public BuffMultipicitive(double amount)
        {
            Bonus = amount;
        }
    }

    public class BuffCalculator
    {
        private readonly int _min;
        private readonly int _max;

        private List<BuffFlat> _flatBuffs = new List<BuffFlat>();
        private List<BuffAdditive> _additiveBuffs = new List<BuffAdditive>();
        private List<BuffMultipicitive> _multipicitiveBuffs = new List<BuffMultipicitive>();

        public void Add(IStatBuff buff)
        {
            if (buff is BuffFlat buffFlat)
            {
                _flatBuffs.Add(buffFlat);
            }
            else if (buff is BuffAdditive buffAdditive)
            {
                _additiveBuffs.Add(buffAdditive);
            }
            else if (buff is BuffMultipicitive buffMultipicitive)
            {
                _multipicitiveBuffs.Add(buffMultipicitive);
            }
            else
            {
                throw new ArgumentException($"Unknown stat buff type {buff}");
            }
        }

        public void AddRange(IEnumerable<IStatBuff> buffs)
        {
            foreach (IStatBuff buff in buffs)
            {
                Add(buff);
            }
        }

        public BuffCalculator(int min = 0, int max = int.MaxValue)
        {
            _min = min;
            _max = max;
        }

        public int Calculate(int number)
        {
            double result = number;
            result = _flatBuffs.Aggregate(result, (last, buff) => last + buff.Bonus);
            result += result * _additiveBuffs.Sum(buff => buff.Bonus);
            result *= _multipicitiveBuffs.Aggregate(1d, (last, buff) => last * buff.Bonus);
            return (int)Math.Round(result);
        }
    }
}
