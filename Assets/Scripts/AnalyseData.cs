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
        bomb = 11 << 13,
        rocket = 11 << 14
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
    }
    class TypeInfo {
        public CardType type { get; set; }
        public Dictionary<CardType, CardValue[]> mainValue = new Dictionary<CardType, CardValue[]>();
        public List<CardValue> postfix = new List<CardValue>();
        public List<byte> byteDatas = new List<byte>();
        public byte GetCount(CardType type) {
            return (byte)(Math.Abs(mainValue[type][1] - mainValue[type][0]) + 1);
        }

        public TypeInfo() {
            type = CardType.single;
            mainValue.Add(CardType.sequence, new CardValue[2]);
            mainValue.Add(CardType.sequencePair, new CardValue[2]);
            mainValue.Add(CardType.sequenceThree, new CardValue[2]);

        }
    }
    static class Utility {
        static byte GetSequenceRequireCount(CardType type) {
            switch (type) {
                case CardType.sequence:
                    return ConstData.singleRequireCount;
                case CardType.sequencePair:
                    return ConstData.pairRequireCount;
                case CardType.sequenceThree:
                    return ConstData.threeRequireCount;
            }
            return 1;
        }
        static byte GetSequenceRequireLength(CardType type) {
            switch (type) {
                case CardType.sequence:
                    return ConstData.sequenceRequireLength;
                case CardType.sequencePair:
                    return ConstData.sequencePairRequireLength;
                case CardType.sequenceThree:
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
            sequenceStart = ConstData.minCardValue;
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
            return (CardValue)(byteData & 0xf);
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
        public static byte[] GenerateData() {
            byte[] cards = {
                0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9, 0xa, 0xb, 0xc, 0xd,
                0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x1a, 0x1b, 0x1c, 0x1d,
                0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27, 0x28, 0x29, 0x2a, 0x2b, 0x2c, 0x2d,
                0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3a, 0x3b, 0x3c, 0x3d,
                0x4e, 0x4f
            };
            System.Random rnd = new System.Random();
            byte[] datas = new byte[20];
            for (int i = 0; i < 20; i++) {
                datas[i] = cards[rnd.Next(0, 54)];
            }
            return datas;
        }
    }
}
