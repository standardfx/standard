using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using Standard.StringMetrics;

namespace Standard.StringMetrics.Tests
{
    public class QGramsDistanceFixture : IDisposable
    {
        public QGramsDistance TrigramExtendedDistance;
        public QGramsDistance TrigramDistance;
        public QGramsDistance BigramExtendedDistance;
        public QGramsDistance BigramDistance;

        public List<QGramsTestRecord> TestNames = new List<QGramsTestRecord>(26);

        public QGramsDistanceFixture()
        {
            LoadData();

            TrigramExtendedDistance = new QGramsDistance();
            TrigramDistance = new QGramsDistance(new TokenizerQGram3());
            BigramExtendedDistance = new QGramsDistance(new TokenizerQGram2Extended());
            BigramDistance = new QGramsDistance(new TokenizerQGram2());
        }

        public void Dispose()
        {
            // do nothing
        }

        private void LoadData()
        {
            AddNames(RS.BlockDistance1);
            AddNames(RS.BlockDistance2);
            AddNames(RS.BlockDistance3);            
        }

        private void AddNames(string addChars) 
        {
            if (addChars != null) 
            {
                string[] letters = addChars.Split(',');
                QGramsTestRecord testName = new QGramsTestRecord(
                    letters[0], letters[1], 
                    Convert.ToDouble(letters[12]), Convert.ToDouble(letters[7]),
                    Convert.ToDouble(letters[8]), Convert.ToDouble(letters[9]));

                TestNames.Add(testName);
            }
        }
    }

    public struct QGramsTestRecord 
    {
        private string nameOne;
        private string nameTwo;
        private double trigramExtendedDistanceMatchLevel;
        private double trigramDistanceMatchLevel;
        private double bigramExtendedDistanceMatchLevel;
        private double bigramDistanceMatchLevel;

        public QGramsTestRecord(string firstName, string secondNamem, double scoreOne, double scoreTwo, double scoreThree, double scoreFour) 
        {
            nameOne = firstName;
            nameTwo = secondNamem;
            trigramExtendedDistanceMatchLevel = scoreOne;
            trigramDistanceMatchLevel = scoreTwo;
            bigramExtendedDistanceMatchLevel = scoreThree;
            bigramDistanceMatchLevel = scoreFour;
        }

        public override string ToString() 
        {
            return (
                NameOne + " : " + 
                NameTwo + " : " + 
                TrigramExtendedDistanceMatchLevel + " : " + 
                TrigramDistanceMatchLevel + " : " + 
                BigramExtendedDistanceMatchLevel + " : " + 
                BigramDistanceMatchLevel
            );
        }

        public string NameOne 
        { 
            get { return nameOne; } 
            set { nameOne = value; } 
        }
        
        public string NameTwo 
        { 
            get { return nameTwo; } 
            set { nameTwo = value; } 
        }

        public double TrigramExtendedDistanceMatchLevel 
        { 
            get { return trigramExtendedDistanceMatchLevel; } 
            set { trigramExtendedDistanceMatchLevel = value; } 
        }
        
        public double TrigramDistanceMatchLevel 
        { 
            get { return trigramDistanceMatchLevel; } 
            set { trigramDistanceMatchLevel = value; } 
        }
        
        public double BigramExtendedDistanceMatchLevel 
        { 
            get { return bigramExtendedDistanceMatchLevel; } 
            set { bigramExtendedDistanceMatchLevel = value; } 
        }
        
        public double BigramDistanceMatchLevel 
        { 
            get { return bigramDistanceMatchLevel; } 
            set { bigramDistanceMatchLevel = value; } 
        }
    }

    public class QGramsDistanceTests : IClassFixture<QGramsDistanceFixture>
    {
        QGramsDistanceFixture fixture;

        public QGramsDistanceTests(QGramsDistanceFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void TrigramExtendedDistance_TestData()
        {
            foreach (QGramsTestRecord testRecord in fixture.TestNames) 
            {
                Assert.Equal(
                    testRecord.TrigramExtendedDistanceMatchLevel.ToString("F3"),
                    fixture.TrigramExtendedDistance.GetSimilarity(
                        testRecord.NameOne, testRecord.NameTwo
                    ).ToString("F3")
                );
            }
        }

        [Fact]
        public void TrigramDistance_TestData()
        {
            foreach (QGramsTestRecord testRecord in fixture.TestNames) 
            {
                Assert.Equal(
                    testRecord.TrigramDistanceMatchLevel.ToString("F3"),
                    fixture.TrigramDistance.GetSimilarity(testRecord.NameOne, testRecord.NameTwo).ToString("F3")
                );
            }
        }

        [Fact]
        public void BigramExtendedDistance_TestData()
        {
            foreach (QGramsTestRecord testRecord in fixture.TestNames) 
            {
                Assert.Equal(
                    testRecord.BigramExtendedDistanceMatchLevel.ToString("F3"),
                    fixture.BigramExtendedDistance.GetSimilarity(testRecord.NameOne, testRecord.NameTwo).ToString("F3")
                );
            }
        }

        [Fact]
        public void BigramDistance_TestData()
        {
            foreach (QGramsTestRecord testRecord in fixture.TestNames) 
            {
                Assert.Equal(
                    testRecord.BigramDistanceMatchLevel.ToString("F3"),
                    fixture.BigramDistance.GetSimilarity(testRecord.NameOne, testRecord.NameTwo).ToString("F3")
                );
            }
        }
    }    
}