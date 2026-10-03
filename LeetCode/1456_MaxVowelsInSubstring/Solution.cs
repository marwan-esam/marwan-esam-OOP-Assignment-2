public class Solution {
    public bool isVowel(char c) {
        return c == 'a' ||
               c == 'e' ||
               c == 'i' ||
               c == 'o' ||
               c == 'u';
    }
    public int MaxVowels(string s, int k) {
        int vowels = 0;
        int maxVowels = 0;
        int start = 0;
        for(int end = 0 ; end < s.Length ; end++) {
            if(end - start + 1 <= k) {
                if(isVowel(s[end])) vowels++;
            } else {
                if(isVowel(s[start])) vowels--;
                if(isVowel(s[end])) vowels++;
                start++;
            }
            maxVowels = vowels > maxVowels ? vowels : maxVowels;
        } 
        return maxVowels;
    }
}
