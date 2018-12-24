using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace AnalyseCards {
    class AnalyseType {
        public TypeInfo GetTypeInfo(byte[] byteDatas) {
            if (byteDatas == null || byteDatas.Length < 1) {
                return typeInfo;
            }
            Utility.PrepareDatas(byteDatas, out datas);
            onlyTypeInfo = Utility.GetOnlyTypeInfo(datas);
            GetSinglePairThreeBomb();
            GetThreeSinglePair();
            GetBombSinglePair();
            GetRocket();
            GetSequence(CardType.sequence);
            GetSequence(CardType.sequencePair);
            GetSequence(CardType.sequenceThree);
            return typeInfo;
        }

        SortedDictionary<CardValue, List<byte>> datas;
        Dictionary<CardType, List<CardValue>> onlyTypeInfo;
        TypeInfo typeInfo = new TypeInfo();
        public override string ToString() {
            string str = "\r\nonlyTypeInfo:\r\n";
            foreach (KeyValuePair<CardType, List<CardValue>> item in onlyTypeInfo) {
                str += item.Key.ToString() + ":\r\n";
                foreach (CardValue cardValue in item.Value) {
                    str += cardValue + ",\t";
                }
                str += "\r\n";
            }
            str += typeInfo.ToString();
            return str;
        }
        void GetSinglePairThreeBomb() {
            if (datas.Count == 1) {
                CardType type = CardType.none;
                if (onlyTypeInfo[CardType.single].Count == 1) {
                    type = CardType.single;
                } else if (onlyTypeInfo[CardType.pair].Count == 1) {
                    type = CardType.pair;
                } else if (onlyTypeInfo[CardType.three].Count == 1) {
                    type = CardType.three;
                } else if (onlyTypeInfo[CardType.bomb].Count == 1) {
                    type = CardType.bomb;
                }
                typeInfo.type = type;
                typeInfo.mainValue = onlyTypeInfo[type][0];
            }
        }
        void GetThreeSinglePair() {
            CardType type = CardType.none;
            if (datas.Count == 2 && onlyTypeInfo[CardType.three].Count == 1) {
                if (onlyTypeInfo[CardType.single].Count == 1) {
                    type = CardType.threeSingle;
                    typeInfo.postfix.Add(onlyTypeInfo[CardType.single][0]);
                } else if (onlyTypeInfo[CardType.pair].Count == 1) {
                    type = CardType.threePair;
                    typeInfo.postfix.Add(onlyTypeInfo[CardType.pair][0]);
                }
                typeInfo.type = type;
                typeInfo.mainValue = onlyTypeInfo[CardType.three][0];
            }
        }
        void GetBombSinglePair() {
            if (datas.Count == 3 && onlyTypeInfo[CardType.bomb].Count == 1) {
                typeInfo.mainValue = onlyTypeInfo[CardType.bomb][0];
                if (onlyTypeInfo[CardType.single].Count == 2) {
                    typeInfo.type = CardType.bombSingle;
                    typeInfo.postfix.AddRange(onlyTypeInfo[CardType.single]);
                }
                if (onlyTypeInfo[CardType.pair].Count == 2) {
                    typeInfo.type = CardType.bombPair;
                    typeInfo.postfix.AddRange(onlyTypeInfo[CardType.pair]);
                }
            }
        }
        void GetRocket() {
            if (datas.Count == 2 &&
            datas.ContainsKey(CardValue.blackJoker) &&
                datas.ContainsKey(CardValue.redJoker)) {
                typeInfo.type = CardType.rocket;
            }
        }
        void GetSequence(CardType type) {
            CardValue sequenceStart, sequenceEnd;
            bool isHaveSequence = 
            Utility.GetSequenceInfo(datas, type, ConstData.minCardValue, out sequenceStart, out sequenceEnd);

            if (isHaveSequence) {
                typeInfo.sequenceData[type].Start = sequenceStart;
                typeInfo.sequenceData[type].End = sequenceEnd;
                byte count = typeInfo.Count(type);
                if (type == CardType.sequenceThree && datas.Count == count * 2) {
                    if (onlyTypeInfo[CardType.single].Count == count) {
                        typeInfo.type = CardType.sequenceThreeSingle;
                        typeInfo.postfix.AddRange(onlyTypeInfo[CardType.single]);
                    } else if (onlyTypeInfo[CardType.pair].Count == count) {
                        typeInfo.type = CardType.sequenceThreePair;
                        typeInfo.postfix.AddRange(onlyTypeInfo[CardType.pair]);
                    }
                } else if (datas.Count == count) {
                    typeInfo.type = type;
                }
            }
        }

    }

}