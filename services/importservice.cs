using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EnglishLearningApp.Models;

namespace EnglishLearningApp.Services
{
    public static class ImportService
    {
        /// <summary>
        /// Import words from a text file
        /// Format: word|definition|example OR just word (one per line)
        /// </summary>
        public static List<WordEntry> ImportFromText(string filepath)
        {
            var entries = new List<WordEntry>();

            if (!File.Exists(filepath))
                throw new FileNotFoundException($"File not found: {filepath}");

            try
            {
                var lines = File.ReadAllLines(filepath);

                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    var parts = line.Split('|');

                    if (parts.Length >= 1)
                    {
                        entries.Add(new WordEntry
                        {
                            Text = parts[0].Trim(),
                            Definition = parts.Length > 1 ? parts[1].Trim() : "[Definition pending]",
                            Example = parts.Length > 2 ? parts[2].Trim() : null,
                            Category = parts.Length > 3 ? parts[3].Trim() : "General",
                            DateAdded = DateTime.Now
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error importing file: {ex.Message}", ex);
            }

            return entries;
        }

        /// <summary>
        /// Import from CSV format
        /// </summary>
        public static List<WordEntry> ImportFromCsv(string filepath)
        {
            return ImportFromText(filepath); // Same format for now
        }
    }
}
