# 📒 Annuaire BST - Phone Book using Binary Search Tree

A C# console application implementing a phone book using a **Binary Search Tree (BST)** as the primary data structure. Contacts are automatically sorted by name. Built in two versions: a standard BST (V1) and an advanced Hybrid Structure (V2).

Developed in collaboration with [Xuefeng Hu (NexPathArchitect)](https://github.com/NexPathArchitect).

## 🎯 Features

### V1 - Standard BST
- Add, search, and delete contacts
- 4 tree traversals: InOrder, PreOrder, PostOrder, BFS
- Search by name, city, and phone number
- Graphical tree display using the **Reingold-Tilford algorithm**
- Execution time benchmarking
- CSV import/export (saved in PreOrder to preserve tree shape on reload)
- Input validation (email, phone number format)
- Duplicate detection and management

### V2 - Hybrid Structure
Three hybrid implementations to explore performance trade-offs:

| Structure | Description |
|---|---|
| BST / BST | 3 separate BSTs indexed by name, city, and phone |
| BST / Hash Table | BST + Hash Table for O(1) phone lookups |
| BST / List | Each node holds a list to handle duplicate names |

## 🧠 Key challenges & solutions

- **Tiebreakers in comparators** - Prevented false duplicates when contacts share the same name
- **Synchronization across 5 structures** - All insertions and deletions update every data structure consistently
- **Reingold-Tilford algorithm** - Implemented graphical tree display without node overlap
- **PreOrder CSV export** - Tree shape is preserved on file reload, avoiding rebalancing issues

## 🔗 Connection to Machine Learning

This project provided hands-on experience with tree structures that directly applies to understanding **Decision Trees** 
in machine learning: both use the same recursive node traversal, root-to-leaf paths, and branching logic - 
the difference being that BSTs search for a value while Decision Trees predict a category.

## 🛠️ Built with

- **Language:** C# (.NET 8)
- **IDE:** Visual Studio
- **Data structures:** BST, Hash Table, Linked List
- **Algorithm:** Reingold-Tilford (tree visualization)

## 🚀 Getting started

1. Clone the repository:
```
git clone https://github.com/IsadoraEaston/annuaire-bst.git
```
2. Open the `.slnx` file in Visual Studio (V1 or V2)
3. Build and run - no external dependencies required

## 👩‍💻 Authors

- **Isabelle D. Easton** - [GitHub](https://github.com/IsadoraEaston)
- **Xuefeng Hu** - [GitHub](https://github.com/NexPathArchitect)
