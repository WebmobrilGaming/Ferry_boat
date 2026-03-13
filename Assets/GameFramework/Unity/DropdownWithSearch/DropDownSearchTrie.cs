using System.Collections.Generic;
namespace GF
{
    public class TrieNode
    {
        public Dictionary<char, TrieNode> children = new();
        public bool isEndOfWord;
        public string fullWord; // store original word
    }

    public class DropDownSearchTrie
    {
        private TrieNode root;

        public DropDownSearchTrie()
        {
            root = new TrieNode();
        }

        public void Insert(string word)
        {
            if (string.IsNullOrEmpty(word))
                return;

            TrieNode current = root;
            foreach (char c in word.ToLowerInvariant())
            {
                if (!current.children.ContainsKey(c))
                    current.children[c] = new TrieNode();

                current = current.children[c];
            }
            current.isEndOfWord = true;
            current.fullWord = word; // keep original casing
        }



        public List<string> Search(string prefix)
        {
            List<string> results = new();
            if (string.IsNullOrEmpty(prefix))
                return results;

            TrieNode current = root;
            foreach (char c in prefix.ToLowerInvariant())
            {
                if (!current.children.TryGetValue(c, out current))
                    return results;
            }

            FindWordsFromNode(current, results);
            return results;
        }


        private void FindWordsFromNode(TrieNode node, List<string> results, int limit = 20)
        {
            if (results.Count >= limit)
                return;

            if (node.isEndOfWord)
                results.Add(node.fullWord);

            foreach (var child in node.children.Values)
                FindWordsFromNode(child, results, limit);
        }
        public void Clear()
        {
            root = new TrieNode();
        }
    }
}
