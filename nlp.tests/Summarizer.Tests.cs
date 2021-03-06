﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using Xunit;
using Moq;

using Microsoft.Extensions.Logging;

using nlp.data;
using nlp.services.text;

namespace nlp.tests
{
    public class SummarizerTests
    {
        private readonly ISummarizer _summarizer;

        public SummarizerTests()
        {
            _summarizer = new Helper<ISummarizer>().GetService();
        }

        [Theory]
        [InlineData(@"In an attempt to build an AI-ready workforce, Microsoft announced Intelligent Cloud Hub which has been launched to empower the next generation of students with AI-ready skills. Envisioned as a three-year collaborative program, Intelligent Cloud Hub will support around 100 institutions with AI infrastructure, course content and curriculum, developer support, development tools and give students access to cloud and AI services. As part of the program, the Redmond giant which wants to expand its reach and is planning to build a strong developer ecosystem in India with the program will set up the core AI infrastructure and IoT Hub for the selected campuses. The company will provide AI development tools and Azure AI services such as Microsoft Cognitive Services, Bot Services and Azure Machine Learning.According to Manish Prakash, Country General Manager-PS, Health and Education, Microsoft India, said, ""With AI being the defining technology of our time, it is transforming lives and industry and the jobs of tomorrow will require a different skillset.This will require more collaborations and training and working with AI.That’s why it has become more critical than ever for educational institutions to integrate new cloud and AI technologies.The program is an attempt to ramp up the institutional set - up and build capabilities among the educators to educate the workforce of tomorrow."" The program aims to build up the cognitive skills and in-depth understanding of developing intelligent cloud connected solutions for applications across industry. Earlier in April this year, the company announced Microsoft Professional Program In AI as a learning track open to the public. The program was developed to provide job ready skills to programmers who wanted to hone their skills in AI and data science with a series of online courses which featured hands-on labs and expert instructors as well. This program also included developer-focused AI school that provided a bunch of assets to help build AI skills.")]
        public void Summarize_Valid(string Content)
        {
            var summ = _summarizer.Summarize(Content);

            Assert.False(string.IsNullOrWhiteSpace(summ));
        }

        [Theory]
        [InlineData("One sentence. Two sentences. And a 3.")]
        [InlineData("I'm not afraid. I'm not surprised. I'm not entitled")]
        [InlineData("The big. Bad. Wolf. Came for me")]
        public void ToSentences_Valid(string Content)
        {
            var sentences = _summarizer.ToSentences(Content)
                .ToArray();

            Assert.NotEmpty(sentences);
        }

        [Theory]
        [InlineData("Sentences with multiple   spaces. In  some    of them.")]
        [InlineData("The big   space in this. Is very        big.")]
        public void ToSentences_WithMultipleSpaces(string Content)
        {
            var sentences = _summarizer.ToSentences(Content).ToArray();

            Assert.DoesNotContain("   ", sentences[0]);
            Assert.DoesNotContain("  ", sentences[1]);
        }

        [Theory]
        [InlineData("Julie loves me more than Linda loves me", "Jane likes me more than Julie loves me", "This is another test", "Let's test another similarity")]
        [InlineData("Test me", "Test me as well", "Test me as well please", "What about me?")]
        public void PageRank_Valid(params string[] Sentences)
        {
            var matrix = _summarizer.BuildSimilarityMatrix(Sentences);
            var pageRanks = _summarizer.PageRank(matrix);

            Assert.NotEmpty(pageRanks);
            Assert.True(pageRanks.First() > 0);
        }

        [Theory]
        [InlineData("Julie loves me more than Linda loves me", "Jane likes me more than Julie loves me", "This is another test", "Let's test another similarity")]
        [InlineData("Test me", "Test me as well", "Test me as well please", "What about me?")]
        public void BuildSimilarityMatrix_Similar(params string[] Sentences)
        {
            var matrix = _summarizer.BuildSimilarityMatrix(Sentences);

            Assert.NotNull(matrix);
            Assert.True(matrix[0, 1] > 0);
            Assert.True(matrix[1, 0] > 0);
        }

        [Theory]
        [InlineData("In an attempt to build an AI-ready workforce, Microsoft announced Intelligent Cloud Hub which has been launched to empower the next generation of students with AI-ready skills.", "Envisioned as a three-year collaborative program, Intelligent Cloud Hub will support around 100 institutions with AI infrastructure, course content and curriculum, developer support, development tools and give students access to cloud and AI services.")]
        [InlineData("Julie loves me more than Linda loves me", "Jane likes me more than Julie loves me")]
        [InlineData("This is another test", "Let's test another similarity")]
        public void SentenceSimilarity_Similar(string Sentence1, string Sentence2)
        {
            var similarity = _summarizer.SentenceSimilarity(Sentence1, Sentence2);

            Assert.True(similarity > 0);
        }

        [Theory]
        [InlineData("In an attempt to build an AI-ready workforce, Microsoft announced Intelligent Cloud Hub which has been launched to empower the next generation of students with AI-ready skills.", "Envisioned as a three-year collaborative program, Intelligent Cloud Hub will support around 100 institutions with AI infrastructure, course content and curriculum, developer support, development tools and give students access to cloud and AI services.")]
        [InlineData("Julie loves me more than Linda loves me", "Jane likes me more than Julie loves me")]
        [InlineData("This is another test", "Let's test another similarity")]
        public void SentenceSimilarity2_Similar(string Sentence1, string Sentence2)
        {
            var sim1 = _summarizer.SentenceSimilarity(Sentence1, Sentence2);
            var sim2 = _summarizer.SentenceSimilarity2(Sentence1, Sentence2);

            Assert.True(sim1 > 0);
            Assert.True(sim2 > 0);
        }

        [Theory]
        [InlineData("this", "is")]
        [InlineData("dog", "similardog")]
        [InlineData("house", "household")]
        [InlineData("yard", "yardwork")]
        public void WordSimilarity_Similar(string Word1, string Word2)
        {
            var similairty = _summarizer.WordSimilarity(Word1, Word2);

            Assert.True(similairty > 0);
        }

        [Theory]
        [InlineData("test", "test")]
        [InlineData("dog", "dog")]
        [InlineData("house", "house")]
        [InlineData("yard", "yard")]
        public void WordSimilarity_Identical(string Word1, string Word2)
        {
            var similairty = _summarizer.WordSimilarity(Word1, Word2);

            Assert.Equal(1.0, Math.Round(similairty, 0));
        }

        [Theory]
        [InlineData(1, 2, 0, 1, 0, 0)]
        [InlineData(1, 1, 1, 0, 0, 0, 0, 1)]
        [InlineData(1, 2, 3, 1, 2, 3)]
        [InlineData(1, 1, 1, 1, 1, 2)]
        public void CosineDistance_BiggerThanZero(params int[] Vector)
        {
            var vector1 = Vector.Take(Vector.Length / 2).ToArray();
            var vector2 = Vector.Skip(Vector.Length / 2).ToArray();

            var cosineDistance = _summarizer.CosineDistance(vector1, vector2);

            Assert.True(cosineDistance >= 0);
        }
    }
}
