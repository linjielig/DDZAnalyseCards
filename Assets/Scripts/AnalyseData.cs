using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace AnalyseCards {
    enum CardValue {
        three = 3,
        four = 4,
        five = 5,
        six = 6,
        seven = 7,
        eight = 8,
        nine = 9,
        ten = 10,
        jack = 11,
        queen = 12,
        king = 13,
        ace = 14,
        two = 15,
        blackJoker = 16,
        redJoker = 17
    }

    [Flags]
    enum CardType {
        none = 1,
        single = 1 << 1,
        pair = 1 << 2,
        three = 1 << 3,
        threeSingle = 1 << 4,
        threePair = 1 << 5,
        bombSingle = 1 << 6,
        bombPair = 1 << 7,
        sequence = 1 << 8,
        sequencePair = 1 << 9,
        sequenceThree = 1 << 10,
        sequenceThreeSingle = 1 << 11,
        sequenceThreePair = 1 << 12,
        bomb = 1 << 13,
        rocket = 1 << 14
    }
    static class ConstData {
        public const CardValue minCardValue = CardValue.three;
        public const CardValue maxCardValue = CardValue.redJoker;
        public const CardValue maxSequenceValue = CardValue.ace;
        public const byte singleRequireCount = 1;
        public const byte pairRequireCount = 2;
        public const byte threeRequireCount = 3;
        public const byte bombRequireCount = 4;
        public const byte sequenceRequireLength = 5;
        public const byte sequencePairRequireLength = 3;
        public const byte sequenceThreeRequireLength = 2;
        public const byte suitCount = 13;
    }
    class SequenceData : IComparable<SequenceData> {
        public CardValue Start { get; set; }
        public CardValue End { get; set; }
        public byte Count() {
            byte count = (byte)Math.Abs(End - Start);
            if (count > 0) {
                return (byte)(count + 1);
            } else {
                return count;
            }
        }
        public int CompareTo(SequenceData d) {
            if (End > d.End) {
                if (Count() >= d.Count()) {
                    return 1;
                }
            } else if (End == d.End && Start == d.Start) {
                return 0;
            }
            return -1;
        }
        public override string ToString() {
            string str = "\r\nStart:\t" + Start + ",\tEnd:\t" + End + ",\tCount:\t" + Count() + "\r\n";
            return str;
        }
    }
    class TypeInfo : MonoBehaviour {
        public CardType type { get; set; }
        public CardValue mainValue { get; set; }
        public Dictionary<CardType, SequenceData> sequenceData = new Dictionary<CardType, SequenceData>();
        public List<CardValue> postfix = new List<CardValue>();
        public List<byte> byteDatas = new List<byte>();
        public byte Count(CardType type) {
            return sequenceData[type].Count();
        }
        public TypeInfo() {
            type = CardType.none;
            sequenceData.Add(CardType.sequence, new SequenceData());
            sequenceData.Add(CardType.sequencePair, new SequenceData());
            sequenceData.Add(CardType.sequenceThree, new SequenceData());
        }
        public int CompareTo(TypeInfo info, CardType type) {
            CardType sequenceType = CardType.sequence | CardType.sequencePair | CardType.sequenceThree | CardType.sequenceThreeSingle | CardType.sequenceThreePair;
            if (Utility.IsAnyOfType(info, sequenceType)) {
                if (Utility.IsContainType(info, type)) {
                    return sequenceData[type].CompareTo(info.sequenceData[type]);
                }
            }
            return -1;
        }
        public override string ToString() {
            string str = "\r\nmainValue:\t" + mainValue;
            str += "\r\nType:\t" + type + "\r\n";
            str += "\r\nbyteDatas:\r\n";
            for (byte i = 0; i < byteDatas.Count; i++) {
                str += byteDatas[i] + "\t";
            }
            str += "\r\npostfix:\r\n";
            for (byte i = 0; i < postfix.Count; i++) {
                str += postfix[i] + "\t";
            }
            str += "\r\n sequenceData:\r\n";
            foreach (KeyValuePair<CardType, SequenceData> item in sequenceData) {
                str += item.Key + item.Value.ToString();
            }
            return str;
        }
    }
    static class Utility {
        public static byte GetSequenceRequireCount(CardType type) {
            switch (type) {
                case CardType.sequence:
                    return ConstData.singleRequireCount;
                case CardType.sequencePair:
                    return ConstData.pairRequireCount;
                case CardType.sequenceThree:
                case CardType.sequenceThreePair:
                case CardType.sequenceThreeSingle:
                    return ConstData.threeRequireCount;
            }
            return 1;
        }
        public static byte GetTypeCount(CardType type) {
            switch (type) {
                case CardType.single:
                    return ConstData.singleRequireCount;
                case CardType.pair:
                    return ConstData.pairRequireCount;
                case CardType.three:
                    return ConstData.threeRequireCount;
                case CardType.bomb:
                    return ConstData.bombRequireCount;
            }
            return 1;
        }
        public static byte GetSequenceRequireLength(CardType type) {
            switch (type) {
                case CardType.sequence:
                    return ConstData.sequenceRequireLength;
                case CardType.sequencePair:
                    return ConstData.sequencePairRequireLength;
                case CardType.sequenceThree:
                case CardType.sequenceThreeSingle:
                case CardType.sequenceThreePair:
                    return ConstData.sequenceThreeRequireLength;
            }
            return 5;
        }
        public static bool GetSequenceInfo(
            SortedDictionary<CardValue, List<byte>> datas,
            CardType type,
            CardValue startValue,
            out CardValue sequenceStart,
            out CardValue sequenceEnd) {
            byte count = 0;
            sequenceStart = startValue;
            sequenceEnd = sequenceStart;
            byte requireLength = GetSequenceRequireLength(type);
            byte requireCount = GetSequenceRequireCount(type);
            for (CardValue i = startValue; i <= ConstData.maxSequenceValue; i++) {
                if (datas.ContainsKey(i) && datas[i].Count >= requireCount) {
                    count++;
                    if (count == 1) {
                        sequenceStart = i;
                    }
                    sequenceEnd = i;
                } else {
                    if (count >= requireLength) {
                        return true;
                    }
                    return false;
                }
            }
            if (count >= requireLength) {
                return true;
            }
            return false;
        }
        public static bool GetSequenceInfo(
            SortedDictionary<CardValue, TypeInfo> infos,
            CardType type,
            CardValue startValue,
            out CardValue sequenceStart,
            out CardValue sequenceEnd) {
            SortedDictionary<CardValue, List<byte>> datas = new SortedDictionary<CardValue, List<byte>>();
            CardInfoToListByte(infos, datas);
            return GetSequenceInfo(datas, type, startValue, out sequenceStart, out sequenceEnd);
        }
        static void CardInfoToListByte(
            SortedDictionary<CardValue, TypeInfo> infos,
            SortedDictionary<CardValue, List<byte>> datas) {
            foreach (KeyValuePair<CardValue, TypeInfo> item in infos) {
                datas.Add(item.Key, item.Value.byteDatas);
            }
        }
        public static CardValue GetCardValue(byte byteData) {
            byte value = (byte)(byteData & 0xf);
            if (value == 14 || value == 15) {
                value += 2;
            }
            if (value == 1 || value == 2) {
                value += 13;
            }
            return (CardValue)value;
        }
        public static void PrepareDatas(byte[] byteDatas, out SortedDictionary<CardValue, List<byte>> datas) {
            datas = new SortedDictionary<CardValue, List<byte>>();
            for (int i = 0; i < byteDatas.Length; i++) {
                CardValue key = GetCardValue(byteDatas[i]);
                if (!datas.ContainsKey(key)) {
                    datas.Add(key, new List<byte>());
                }
                datas[key].Add(byteDatas[i]);
            }
        }
        public static void PrepareDatas(byte[] byteDatas, out SortedDictionary<CardValue, TypeInfo> infos) {
            SortedDictionary<CardValue, List<byte>> cards;
            PrepareDatas(byteDatas, out cards);
            infos = new SortedDictionary<CardValue, TypeInfo>();
            foreach (KeyValuePair<CardValue, List<byte>> item in cards) {
                TypeInfo info = new TypeInfo();
                info.byteDatas = item.Value;
                info.type = CardType.single;
                info.mainValue = item.Key;
                infos.Add(item.Key, info);
            }
        }
        public static Dictionary<CardType, List<CardValue>> GetOnlyTypeInfo(SortedDictionary<CardValue, List<byte>> datas) {
            Dictionary<CardType, List<CardValue>> onlyTypeInfo = new Dictionary<CardType, List<CardValue>> {
            { CardType.single, new List<CardValue>() },
            { CardType.pair, new List<CardValue>() },
            { CardType.three, new List<CardValue>() },
            { CardType.bomb, new List<CardValue>() }
            };
            CardType type = CardType.none;
            foreach (KeyValuePair<CardValue, List<byte>> item in datas) {
                switch (item.Value.Count) {
                    case ConstData.singleRequireCount:
                        type = CardType.single;
                        break;
                    case ConstData.pairRequireCount:
                        type = CardType.pair;
                        break;
                    case ConstData.threeRequireCount:
                        type = CardType.three;
                        break;
                    case ConstData.bombRequireCount:
                        type = CardType.bomb;
                        break;
                }
                onlyTypeInfo[type].Add(item.Key);
            }
            return onlyTypeInfo;
        }
        public static Dictionary<CardType, List<CardValue>> GetOnlyTypeInfo(SortedDictionary<CardValue, TypeInfo> infos) {
            SortedDictionary<CardValue, List<byte>> datas = new SortedDictionary<CardValue, List<byte>>();
            CardInfoToListByte(infos, datas);
            return GetOnlyTypeInfo(datas);
        }
        public static bool IsContainType(TypeInfo info, CardType type) {
            if ((info.type & type) == type) {
                return true;
            }
            if (type == CardType.sequenceThree && IsAnyOfType(info, CardType.sequenceThreeSingle | CardType.sequenceThreePair)) {
                return true;
            }
            return false;
        }
        public static bool IsAnyOfType(TypeInfo info, CardType type) {
            if ((info.type & type) != 0) {
                return true;
            }
            return false;
        }
        static byte[] cardDatas = {
                0x1,    0x2,    0x3,    0x4,    0x5,    0x6,    0x7,    0x8,    0x9,    0xa,    0xb,    0xc,    0xd,
                0x11,   0x12,   0x13,   0x14,   0x15,   0x16,   0x17,   0x18,   0x19,   0x1a,   0x1b,   0x1c,   0x1d,
                0x21,   0x22,   0x23,   0x24,   0x25,   0x26,   0x27,   0x28,   0x29,   0x2a,   0x2b,   0x2c,   0x2d,
                0x31,   0x32,   0x33,   0x34,   0x35,   0x36,   0x37,   0x38,   0x39,   0x3a,   0x3b,   0x3c,   0x3d,
                0x4e,   0x4f
            };
        public static byte[] GenerateDatas() {

            System.Random rnd = new System.Random();
            byte[] datas = new byte[20];
            for (int i = 0; i < 20; i++) {
                datas[i] = cardDatas[rnd.Next(0, 54)];
            }
            return datas;
        }
        public static byte[] GetSinglePairThreeBombDatas(CardType type) {
            List<byte> byteDatas = new List<byte>();
            for (byte i = 0; i < ConstData.suitCount * GetTypeCount(type); i++) {
                byteDatas.Add(cardDatas[i]);
            }
            return byteDatas.ToArray();
        }
    }
}
