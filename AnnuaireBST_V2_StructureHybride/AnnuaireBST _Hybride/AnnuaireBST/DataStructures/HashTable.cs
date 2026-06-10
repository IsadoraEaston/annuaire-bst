using AnnuaireBST.Models;

namespace AnnuaireBST.DataStructures;

/// <summary>
/// Table de hachage pour recherche O(1) par clé unique (téléphone)
/// Utilise le chainage (liste chainee) pour gerer les collisions
/// Complexite : Add O(1), Get O(1), Remove O(1)
/// </summary>
public class HashTable<K, V>
{
    private LinkedList<(K key, V value)>[] _buckets;
    private int _size;
    private int _count;

    // Constructeur — taille par defaut 101 (nombre premier)
    public HashTable(int size = 101)
    {
        _size = size;
        _buckets = new LinkedList<(K, V)>[size];
        _count = 0;
    }

    // Calcule l'index du bucket pour une cle
    // Complexite : O(1)
    private int GetIndex(K key)
    {
        return Math.Abs(key.GetHashCode()) % _size;
    }

    // Ajoute une paire cle-valeur
    // Complexite : O(1)
    public void Add(K key, V value)
    {
        int index = GetIndex(key);
        if (_buckets[index] == null)
            _buckets[index] = new LinkedList<(K, V)>();

        // Verifier si la cle existe deja
        foreach (var item in _buckets[index])
        {
            if (item.key.Equals(key))
                return; // cle deja presente
        }

        _buckets[index].AddLast((key, value));
        _count++;
    }

    // Recupere la valeur associee a une cle
    // Complexite : O(1)
    public V Get(K key)
    {
        int index = GetIndex(key);
        if (_buckets[index] == null) return default;

        foreach (var item in _buckets[index])
        {
            if (item.key.Equals(key))
                return item.value;
        }
        return default;
    }

    // Supprime une paire cle-valeur
    // Complexite : O(1)
    public void Remove(K key)
    {
        int index = GetIndex(key);
        if (_buckets[index] == null) return;

        var node = _buckets[index].First;
        while (node != null)
        {
            if (node.Value.key.Equals(key))
            {
                _buckets[index].Remove(node);
                _count--;
                return;
            }
            node = node.Next;
        }
    }

    // Verifie si une cle existe
    // Complexite : O(1)
    public bool Contains(K key)
    {
        int index = GetIndex(key);
        if (_buckets[index] == null) return false;

        foreach (var item in _buckets[index])
        {
            if (item.key.Equals(key))
                return true;
        }
        return false;
    }

    // Nombre d'elements
    public int Count => _count;
}