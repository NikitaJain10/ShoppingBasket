# Test Cases

Prices: Apple 35p · Banana 20p · Melon 50p (buy one get one free) · Lime 15p (3 for the price of 2)

## 1. Basic pricing

| ID | Scenario | Input | Expected |
|----|----------|-------|----------|
| TC01 | Empty basket | `[]` | 0p |
| TC02 | Single apple | `[Apple]` | 35p |
| TC03 | Single banana | `[Banana]` | 20p |
| TC04 | Single melon | `[Melon]` | 50p |
| TC05 | Single lime | `[Lime]` | 15p |
| TC06 | Example from brief | `[Apple, Apple, Banana]` | 90p |
| TC07 | Apples, no offer | 2 / 3 / 10 apples | 70p / 105p / 350p |
| TC08 | Bananas, no offer | 2 / 5 bananas | 40p / 100p |

## 2. Melons — buy one get one free

| ID | Quantity | Charged for | Expected |
|----|----------|-------------|----------|
| TC09 | 1 | 1 | 50p |
| TC10 | 2 | 1 | 50p |
| TC11 | 3 | 2 | 100p |
| TC12 | 4 | 2 | 100p |
| TC13 | 5 | 3 | 150p |
| TC14 | 10 | 5 | 250p |

## 3. Limes — three for the price of two

| ID | Quantity | Charged for | Expected |
|----|----------|-------------|----------|
| TC15 | 1 | 1 | 15p |
| TC16 | 2 | 2 | 30p |
| TC17 | 3 | 2 | 30p |
| TC18 | 4 | 3 | 45p |
| TC19 | 5 | 4 | 60p |
| TC20 | 6 | 4 | 60p |
| TC21 | 7 | 5 | 75p |
| TC22 | 9 | 6 | 90p |

## 4. Mixed baskets

| ID | Scenario | Input | Expected |
|----|----------|-------|----------|
| TC23 | All offers together | 2 Apple, 1 Banana, 2 Melon, 3 Lime | 170p |
| TC24 | Same basket, shuffled order | Lime, Melon, Apple, Lime, Banana, Melon, Apple, Lime | 170p |
| TC25 | One of each — no offer triggered | Apple, Banana, Melon, Lime | 120p |
| TC26 | Large mixed basket | 3 Apple, 4 Banana, 5 Melon, 8 Lime | 425p |
| TC27 | Very large basket | 100,000 Melons | 2,500,000p |

## 5. Input handling and errors

| ID | Scenario | Input | Expected |
|----|----------|-------|----------|
| TC28 | Lower case | `[apple]` | 35p |
| TC29 | Upper case | `[APPLE]` | 35p |
| TC30 | Surrounding spaces | `["  Apple  "]` | 35p |
| TC31 | Mixed case counts toward one offer | `[melon, MELON]` | 50p |
| TC32 | Null basket | `null` | `ArgumentNullException` |
| TC33 | Empty / whitespace / null item | `[Apple, ""]` etc. | `ArgumentException` |
| TC34 | Unknown item | `[Apple, Mango]` | `UnknownItemException` (ItemName = "Mango") |
| TC35 | Plural name | `[Apples]` | `UnknownItemException` |
| TC36 | Null catalogue | `new BasketCalculator(null)` | `ArgumentNullException` |

## 6. Receipt breakdown

| ID | Scenario | Expected |
|----|----------|----------|
| TC37 | `[Melon, Apple, Melon, Lime, Lime, Lime]` | Lines in first-seen order (Melon, Apple, Lime); Melon saving 50p; Lime saving 15p; subtotal 180p; savings 65p; total 115p |

## 7. Pricing rules (unit level)

| ID | Scenario | Expected |
|----|----------|----------|
| TC38 | Unit rule: qty × price (0, 1, 4 items) | 0p, 35p, 80p |
| TC39 | BOGOF rule for 0–7 items | charges ⌈qty / 2⌉ items |
| TC40 | 3-for-2 rule for 0–6 items | charges 2 of every 3, plus remainder |
| TC41 | Custom "5 for 3" with 11 items | charged for 7 |
| TC42 | Group size 1, charge 1 | full price |
| TC43 | Invalid rule config (group 0, charged > group, negative) | `ArgumentOutOfRangeException` |
| TC44 | Blank offer description | `ArgumentException` |
| TC45 | Negative quantity / negative price | `ArgumentOutOfRangeException` |
| TC46 | Overflow (`int.MaxValue × 2`) | `OverflowException` |

## 8. Catalogue and formatting

| ID | Scenario | Expected |
|----|----------|----------|
| TC47 | Default catalogue prices | Match the brief |
| TC48 | Default catalogue offers | Melon = 2-for-1, Lime = 3-for-2, Apple = none |
| TC49 | Unknown product lookup | `false`, product null |
| TC50 | Duplicate product names (any case) | `ArgumentException` |
| TC51 | Calculator with custom catalogue (Orange BOGOF ×3) | 80p |
| TC52 | Product with blank name / negative price / null rule | Exception |
| TC53 | Price formatting | 0→"0p", 99→"99p", 100→"£1.00", 170→"£1.70", 12345→"£123.45", -50→"-50p" |
