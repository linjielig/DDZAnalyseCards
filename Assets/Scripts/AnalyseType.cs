using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace AnalyseCards {
    class AnalyseType {
        public TypeInfo GetTypeInfo(byte[] datas) {
            if (datas == null || datas.Length < 1) {
                return typeInfo;
            }
            Utility.PrepareDatas(datas, out cards);
            GetSingleOrPairInfo();
            GetThreeInfo();
            GetBombInfo();
            GetRocketInfo();
            GetSequenceInfo(CardType.sequence);
            GetSequenceInfo(CardType.sequencePair);
            GetSequenceInfo(CardType.sequenceThree);
            return typeInfo;
        }

        SortedDictionary<CardValue, List<byte>> cards = null;
        TypeOnlyInfo typeonlyInfo = new TypeOnlyInfo();
        TypeInfo typeInfo = new TypeInfo();

        void GetSingleOrPairInfo() {
            if (cards.Count == 1) {
                if (typeonlyInfo.singleKeys.Count == 1) {
                    typeInfo.type = CardType.single;
                    typeInfo.mainValue[0] = typeonlyInfo.singleKeys[0];
                }
                if (typeonlyInfo.pairKeys.Count == 1) {
                    typeInfo.type = CardType.pair;
                    typeInfo.mainValue[0] = typeonlyInfo.pairKeys[0];
                }
            }
        }
        void GetThreeInfo() {
            if (typeonlyInfo.threeKeys.Count == 1) {
                typeInfo.mainValue[0] = typeonlyInfo.threeKeys[0];

                if (cards.Count == 1) {
                    typeInfo.type = CardType.three;
                }
                if (cards.Count == 2) {
                    if (typeonlyInfo.singleKeys.Count == 1) {
                        typeInfo.type = CardType.threeSingle;
                        typeInfo.prefixValue[0] = typeonlyInfo.singleKeys[0];

                    }
                    if (typeonlyInfo.pairKeys.Count == 1) {
                        typeInfo.type = CardType.threePair;
                        typeInfo.prefixValue[0] = typeonlyInfo.pairKeys[0];
                    }
                }
            }
        }
        void GetBombInfo() {
            if (typeonlyInfo.bombKeys.Count == 1) {
                typeInfo.mainValue[0] = typeonlyInfo.bombKeys[0];

                if (cards.Count == 1) {
                    typeInfo.type = CardType.bomb;
                }
                if (cards.Count == 3) {
                    if (typeonlyInfo.singleKeys.Count == 2) {
                        typeInfo.type = CardType.bombSingle;
                        typeInfo.prefixValue[0] = typeonlyInfo.singleKeys[0];
                        typeInfo.prefixValue[1] = typeonlyInfo.singleKeys[1];
                    }
                    if (typeonlyInfo.pairKeys.Count == 2) {
                        typeInfo.type = CardType.bombPair;
                        typeInfo.prefixValue[0] = typeonlyInfo.pairKeys[0];
                        typeInfo.prefixValue[1] = typeonlyInfo.pairKeys[1];
                    }
                }
            }
        }

        void GetRocketInfo() {
            if (cards.Count == 2 &&
            cards.ContainsKey(CardValue.blackJoker) &&
                cards.ContainsKey(CardValue.redJoker)) {
                typeInfo.type = CardType.rocket;
            }
        }

        void GetSequenceInfo(CardType type) {
            CardValue sequenceStart, sequenceEnd;
            bool isHaveSequence = false;
            Utility.GetSequenceInfo(cards, type, ConstData.minCardValue, out sequenceStart, out sequenceEnd);

            if (isHaveSequence) {
                typeInfo.mainValue[0] = sequenceStart;
                typeInfo.mainValue[1] = sequenceEnd;
                byte count = (byte)(Math.Abs(sequenceEnd - sequenceStart) + 1);
                if (type == CardType.sequenceThree && count != cards.Count) {
                    if (typeonlyInfo.single.keys.Count == count && cards.Count == count * 2) {
                        for (byte i = 0; i < typeonlyInfo.single.keys.Count; i++) {
                            typeInfo.prefixValue[i] = typeonlyInfo.single.keys[i];
                        }
                        typeInfo.type = CardType.sequenceThreeSingle;
                    }
                    if (typeonlyInfo.pair.keys.Count == count && cards.Count == count * 2) {
                        for (byte i = 0; i < typeonlyInfo.pair.keys.Count; i++) {
                            typeInfo.prefixValue[i] = typeonlyInfo.pair.keys[i];
                        }
                        typeInfo.type = CardType.sequenceThreePair;
                    }
                } else {
                    typeInfo.type = type;
                }
            }
        }

    }

}