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

    class TypeInfo {
        public CardType type = CardType.none;
        // single, pair, three, bomb, rocket 使用mainValue[0]记录牌型大小。
        // sequence, sequencePair, sequenceThree 使用mainValue[0]记录开始，
        // mainValue[1]记录结束。
        public CardValue[] mainValue = new CardValue[2];
        // threeSingle, threePair, bombSingle, bombPair, sequenceThreeSingle,
        // sequenceThreePair 使用mainValue记录牌型大小或开始结束，并用prefix记录带的
        // 牌的大小。
        public CardValue[] prefixValue = new CardValue[4];
    }
    class TypeOnlyInfo {
        public List<CardValue> singleKeys = new List<CardValue>();
        public List<CardValue> pairKeys = new List<CardValue>();
        public List<CardValue> threeKeys = new List<CardValue>();
        public List<CardValue> bombKeys = new List<CardValue>();
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

    class CardInfo {
        public CardType type { get; set; }
        public List<byte> datas = new List<byte>();
        public CardValue SequenceStart { get; set; }
        public CardValue SequenceEnd { get; set; }
        public CardValue PairSequenceStart { get; set; }
        public CardValue PairSequenceEnd { get; set; }
        public CardValue ThreeSequenceStart { get; set; }
        public CardValue ThreeSequenceEnd { get; set; }

        public override string ToString() {
            string str = "\r\ncard type:\t" + type.ToString() + "\r\n" +
                 "\r\nsequence range:\t" + SequenceStart + ",\t" + SequenceEnd + "\r\n" +
                "pair sequence range:\t" + PairSequenceStart + ",\t" + PairSequenceEnd + "\r\n" +
                "three sequence range:\t" + ThreeSequenceStart + ",\t" + ThreeSequenceEnd + "\r\n" +
                "card datas:\r\n";
            for (int i = 0; i < datas.Count; i++) {
                str += datas[i] + "\t";
            }
            str += "\r\n ---------------------------------------------------------->>>>>>>>>>>>>>>>>>>>>>>>>> \r\n";

            return str;
        }
        public CardInfo() {
            type = CardType.single;
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
            SortedDictionary<CardValue, List<byte>> cards,
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
                if (cards.ContainsKey(i) && cards[i].Count >= requireCount) {
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
            SortedDictionary<CardValue, CardInfo> cardInfos,
            CardType type,
            CardValue startValue,
            out CardValue sequenceStart,
            out CardValue sequenceEnd) {
            SortedDictionary<CardValue, List<byte>> cardDatas = new SortedDictionary<CardValue, List<byte>>();
            CardInfoToListByte(cardInfos, cardDatas);
            return GetSequenceInfo(cardDatas, type, startValue, out sequenceStart, out sequenceEnd);
        }
        static void CardInfoToListByte(
            SortedDictionary<CardValue, CardInfo> cardInfos,
            SortedDictionary<CardValue, List<byte>> cardDatas) {
            foreach (KeyValuePair<CardValue, CardInfo> item in cardInfos) {
                cardDatas.Add(item.Key, item.Value.datas);
            }
        }
        public static CardValue GetCardValue(byte data) {
            return (CardValue)(data & 0xf);
        }
        public static void PrepareDatas(byte[] datas, out SortedDictionary<CardValue, List<byte>> cards) {
            cards = new SortedDictionary<CardValue, List<byte>>();
            for (int i = 0; i < datas.Length; i++) {
                CardValue key = GetCardValue(datas[i]);
                if (!cards.ContainsKey(key)) {
                    cards.Add(key, new List<byte>());
                }
                cards[key].Add(datas[i]);
            }
        }
        public static void PrepareDatas(byte[] byteDatas, out SortedDictionary<CardValue, CardInfo> infos) {
            SortedDictionary<CardValue, List<byte>> cards;
            PrepareDatas(byteDatas, out cards);
            infos = new SortedDictionary<CardValue, CardInfo>();
            foreach (KeyValuePair<CardValue, List<byte>> item in cards) {
                CardInfo info = new CardInfo();
                info.datas = item.Value;
                infos.Add(item.Key, info);
            }
        }
        public static TypeOnlyInfo GetTypeInfo(SortedDictionary<CardValue, List<byte>> datas) {
            TypeOnlyInfo typeOnlyInfo = new TypeOnlyInfo();
            foreach (KeyValuePair<CardValue, List<byte>> item in datas) {
                switch (item.Value.Count) {
                    case 1:
                        typeOnlyInfo.singleKeys.Add(item.Key);
                        break;
                    case 2:
                        typeOnlyInfo.pairKeys.Add(item.Key);
                        break;
                    case 3:
                        typeOnlyInfo.threeKeys.Add(item.Key);
                        break;
                    case 4:
                        typeOnlyInfo.bombKeys.Add(item.Key);
                        break;
                }
            }
            return typeOnlyInfo;
        }
        public static TypeOnlyInfo GetTypeInfo(SortedDictionary<CardValue, CardInfo> infos) {
            SortedDictionary<CardValue, List<byte>> datas = new SortedDictionary<CardValue, List<byte>>();
            CardInfoToListByte(infos, datas);
            return GetTypeInfo(datas);
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
