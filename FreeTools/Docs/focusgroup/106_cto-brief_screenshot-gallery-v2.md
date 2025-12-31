# 106 — CTO Brief: Screenshot Gallery Redesign — Round 2

> **Document ID:** 106  
> **Category:** CTO Brief  
> **Purpose:** 5 new layout options combining arrows + captions based on CTO feedback  
> **Date:** 2025-12-31  
> **Related Docs:** `105_cto-brief_screenshot-gallery-redesign.md` (Previous Options)  
> **Resolution:** ⏳ Awaiting CTO decision

---

## CTO Feedback Summary

From 105 review:
- **Likes:** Hybrid B's captions explaining each step
- **Likes:** Hybrid A's arrows showing flow progression
- **Wants:** Something like Hybrid A but WITH captions on the images

---

## 5 New Layout Options

All options focus on the auth section display. Public pages remain in compact 3-column grid.

---

### Option 6: Horizontal Flow with Caption Labels

Arrows between images, captions directly below each screenshot.

```
┌─────────────────────────────────────────────────────────────────────────┐
│ OPTION 6: HORIZONTAL FLOW + CAPTION LABELS                              │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                         │
│  ### 🔐 `/auth` — Login Flow                                            │
│                                                                         │
│  ┌─────────────┐         ┌─────────────┐         ┌─────────────┐       │
│  │             │         │             │         │             │       │
│  │             │         │             │         │             │       │
│  │   [img 1]   │  ────►  │   [img 2]   │  ────►  │   [img 3]   │       │
│  │             │         │             │         │             │       │
│  │             │         │             │         │             │       │
│  └─────────────┘         └─────────────┘         └─────────────┘       │
│   1️⃣ Login Form          2️⃣ Filled Form          3️⃣ Result            │
│   Redirected here        admin@test.com          Invalid credentials   │
│                                                                         │
└─────────────────────────────────────────────────────────────────────────┘

MARKDOWN:

| | | | | |
|:---:|:---:|:---:|:---:|:---:|
| <img src="1-initial.png" width="200"/> | → | <img src="2-filled.png" width="200"/> | → | <img src="3-result.png" width="200"/> |
| **1️⃣ Login Form** | | **2️⃣ Filled Form** | | **3️⃣ Result** |
| *Redirected here* | | *admin@test.com* | | *Invalid credentials* |

✅ Arrows show clear progression
✅ Two-line captions (title + detail)
✅ Compact horizontal layout
⚠️ Table columns may render narrow on mobile
```

---

### Option 7: Stacked Cards with Arrow Connector

Each step is a "card" with image + caption, connected by vertical arrows.

```
┌─────────────────────────────────────────────────────────────────────────┐
│ OPTION 7: STACKED CARDS + VERTICAL ARROWS                               │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                         │
│  ### 🔐 `/auth` — Login Flow                                            │
│                                                                         │
│  ┌─────────────────────────────────────────────────────────────────┐   │
│  │  1️⃣ REDIRECT TO LOGIN                                           │   │
│  │  ┌─────────────────────┐                                        │   │
│  │  │                     │  User navigated to /auth               │   │
│  │  │      [image]        │  Redirected to login page              │   │
│  │  │                     │                                        │   │
│  │  └─────────────────────┘                                        │   │
│  └─────────────────────────────────────────────────────────────────┘   │
│                              │                                          │
│                              ▼                                          │
│  ┌─────────────────────────────────────────────────────────────────┐   │
│  │  2️⃣ CREDENTIALS ENTERED                                         │   │
│  │  ┌─────────────────────┐                                        │   │
│  │  │                     │  Email: admin@test.com                 │   │
│  │  │      [image]        │  Password: ••••••••                    │   │
│  │  │                     │                                        │   │
│  │  └─────────────────────┘                                        │   │
│  └─────────────────────────────────────────────────────────────────┘   │
│                              │                                          │
│                              ▼                                          │
│  ┌─────────────────────────────────────────────────────────────────┐   │
│  │  3️⃣ LOGIN RESULT                                                │   │
│  │  ┌─────────────────────┐                                        │   │
│  │  │                     │  ❌ Invalid credentials                │   │
│  │  │      [image]        │  User remains on login page            │   │
│  │  │                     │                                        │   │
│  │  └─────────────────────┘                                        │   │
│  └─────────────────────────────────────────────────────────────────┘   │
│                                                                         │
└─────────────────────────────────────────────────────────────────────────┘

MARKDOWN:

#### 1️⃣ Redirect to Login
| | |
|:---:|:---|
| <img src="1-initial.png" width="300"/> | User navigated to `/auth`<br/>Redirected to login page |

⬇️

#### 2️⃣ Credentials Entered  
| | |
|:---:|:---|
| <img src="2-filled.png" width="300"/> | Email: `admin@test.com`<br/>Password: `••••••••` |

⬇️

#### 3️⃣ Login Result
| | |
|:---:|:---|
| <img src="3-result.png" width="300"/> | ❌ Invalid credentials<br/>User remains on login page |

✅ Maximum detail per step
✅ Large readable images
✅ Room for detailed captions
⚠️ Takes more vertical space
⚠️ Less "at a glance" than horizontal
```

---

### Option 8: Comic Strip / Storyboard Style

Images in a row with numbered badges and captions in a separate row below.

```
┌─────────────────────────────────────────────────────────────────────────┐
│ OPTION 8: COMIC STRIP / STORYBOARD                                      │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                         │
│  ### 🔐 `/auth` — Login Flow                                            │
│                                                                         │
│  ┌──────────────────────────────────────────────────────────────────┐  │
│  │                                                                   │  │
│  │   ①                      ②                      ③                │  │
│  │  ┌─────────┐ ─────────► ┌─────────┐ ─────────► ┌─────────┐       │  │
│  │  │         │            │ ▓▓▓▓▓▓▓ │            │  ⚠️     │       │  │
│  │  │  Login  │            │ ▓▓▓▓▓▓▓ │            │ Error!  │       │  │
│  │  │  Form   │            │ ▓▓▓▓▓▓▓ │            │         │       │  │
│  │  └─────────┘            └─────────┘            └─────────┘       │  │
│  │                                                                   │  │
│  │  ─────────────────────────────────────────────────────────────── │  │
│  │                                                                   │  │
│  │  ① Redirected to /Account/Login after accessing protected route  │  │
│  │  ② Entered test credentials: admin@test.com / password           │  │
│  │  ③ Login failed: Invalid username or password                    │  │
│  │                                                                   │  │
│  └──────────────────────────────────────────────────────────────────┘  │
│                                                                         │
└─────────────────────────────────────────────────────────────────────────┘

MARKDOWN:

> **🔐 `/auth` — Login Flow**

| ① | → | ② | → | ③ |
|:---:|:---:|:---:|:---:|:---:|
| <img src="1-initial.png" width="180"/> | | <img src="2-filled.png" width="180"/> | | <img src="3-result.png" width="180"/> |

| Step | What Happened |
|:----:|---------------|
| ① | Redirected to `/Account/Login` after accessing protected route |
| ② | Entered test credentials: `admin@test.com` / `password` |
| ③ | Login failed: Invalid username or password |

✅ Clean separation of visuals and narrative
✅ Numbered badges tie images to descriptions
✅ Compact image row + detailed legend
✅ Easy to scan images first, read details second
```

---

### Option 9: Before/After Comparison Style

Emphasizes the transformation from start to end, with middle step as optional detail.

```
┌─────────────────────────────────────────────────────────────────────────┐
│ OPTION 9: BEFORE → AFTER (with middle)                                  │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                         │
│  ### 🔐 `/auth` — Login Flow                                            │
│                                                                         │
│  ┌────────────────────────────┐    ┌────────────────────────────┐      │
│  │        BEFORE              │    │         AFTER              │      │
│  │  ┌──────────────────────┐  │    │  ┌──────────────────────┐  │      │
│  │  │                      │  │    │  │                      │  │      │
│  │  │                      │  │    │  │                      │  │      │
│  │  │     [1-initial]      │  │ ─► │  │     [3-result]       │  │      │
│  │  │                      │  │    │  │                      │  │      │
│  │  │                      │  │    │  │                      │  │      │
│  │  └──────────────────────┘  │    │  └──────────────────────┘  │      │
│  │  Redirected to login       │    │  ❌ Login failed           │      │
│  └────────────────────────────┘    └────────────────────────────┘      │
│                                                                         │
│  <details>                                                              │
│  <summary>📋 See form filled (step 2)</summary>                         │
│                                                                         │
│  │ Credentials Used │                                                   │
│  │ [2-filled.png]   │                                                   │
│  │ admin@test.com   │                                                   │
│                                                                         │
│  </details>                                                             │
│                                                                         │
└─────────────────────────────────────────────────────────────────────────┘

MARKDOWN:

#### 🔐 `/auth` — Login Flow

| Before | → | After |
|:------:|:-:|:-----:|
| <img src="1-initial.png" width="280"/> | ➡️ | <img src="3-result.png" width="280"/> |
| *Redirected to login* | | *❌ Login failed* |

<details>
<summary>📋 Show credentials entered (step 2)</summary>

| Form Filled |
|:-----------:|
| <img src="2-filled.png" width="250"/> |
| `admin@test.com` / `••••••••` |

</details>

✅ Emphasizes start→end outcome
✅ Middle step available but not cluttering
✅ Quick to understand: "tried to access, got rejected"
⚠️ Hides the filled form (might be important)
```

---

### Option 10: Annotated Timeline Strip

Horizontal strip with step markers and a caption bar below.

```
┌─────────────────────────────────────────────────────────────────────────┐
│ OPTION 10: ANNOTATED TIMELINE STRIP                                     │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                         │
│  ### 🔐 `/auth` — Login Flow                                            │
│                                                                         │
│  ══════════════════════════════════════════════════════════════════    │
│       ●────────────────────●────────────────────●                       │
│       │                    │                    │                       │
│  ┌─────────┐          ┌─────────┐          ┌─────────┐                 │
│  │         │          │  ▓▓▓▓   │          │   ⚠️    │                 │
│  │ [img 1] │          │ [img 2] │          │ [img 3] │                 │
│  │         │          │  ▓▓▓▓   │          │         │                 │
│  └─────────┘          └─────────┘          └─────────┘                 │
│       │                    │                    │                       │
│       ▼                    ▼                    ▼                       │
│  ┌─────────┐          ┌─────────┐          ┌─────────┐                 │
│  │Redirect │          │  Fill   │          │ Submit  │                 │
│  │to login │          │  form   │          │ result  │                 │
│  └─────────┘          └─────────┘          └─────────┘                 │
│                                                                         │
│  📝 User navigated to /auth → redirected to login → entered creds →    │
│     submitted → received "invalid credentials" error                    │
│                                                                         │
└─────────────────────────────────────────────────────────────────────────┘

MARKDOWN:

#### 🔐 `/auth` — Login Flow

| Step 1 | | Step 2 | | Step 3 |
|:------:|:-:|:------:|:-:|:------:|
| <img src="1-initial.png" width="180"/> | ● | <img src="2-filled.png" width="180"/> | ● | <img src="3-result.png" width="180"/> |
| **Redirect** | ─ | **Fill Form** | ─ | **Result** |
| *Login page* | | *Credentials* | | *Error* |

> 📝 **Flow:** User navigated to `/auth` → redirected to login → entered credentials → submitted → received "invalid credentials" error

✅ Timeline visual metaphor
✅ Short labels + summary narrative
✅ Professional documentation feel
✅ Good for understanding sequence
```

---

## Side-by-Side Comparison

```
┌─────────────────────────────────────────────────────────────────────────┐
│                        QUICK COMPARISON                                 │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                         │
│  OPTION 6: Horizontal + Captions Below                                  │
│  ┌─────┐ → ┌─────┐ → ┌─────┐                                           │
│  │ img │   │ img │   │ img │   ← Images in row                         │
│  └─────┘   └─────┘   └─────┘                                           │
│  caption   caption   caption   ← Captions below each                   │
│  ─────────────────────────────────────────────────────────────────     │
│                                                                         │
│  OPTION 7: Stacked Cards                                                │
│  ┌─────────────────────────┐                                           │
│  │ ① [img] description     │   ← Each step is a card                   │
│  └─────────────────────────┘                                           │
│              ▼                                                          │
│  ┌─────────────────────────┐                                           │
│  │ ② [img] description     │                                           │
│  └─────────────────────────┘                                           │
│  ─────────────────────────────────────────────────────────────────     │
│                                                                         │
│  OPTION 8: Comic Strip + Legend                                         │
│  ┌─────┐ → ┌─────┐ → ┌─────┐                                           │
│  │ ①   │   │ ②   │   │ ③   │   ← Numbered images                       │
│  └─────┘   └─────┘   └─────┘                                           │
│  ───────────────────────────                                            │
│  ① First thing happened...     ← Legend explains each                  │
│  ② Second thing happened...                                             │
│  ③ Third thing happened...                                              │
│  ─────────────────────────────────────────────────────────────────     │
│                                                                         │
│  OPTION 9: Before/After                                                 │
│  ┌─────────┐       ┌─────────┐                                         │
│  │ BEFORE  │  ───► │  AFTER  │   ← Start and end emphasized            │
│  │ [img 1] │       │ [img 3] │                                         │
│  └─────────┘       └─────────┘                                         │
│  <details>Step 2 hidden</details>                                       │
│  ─────────────────────────────────────────────────────────────────     │
│                                                                         │
│  OPTION 10: Timeline Strip                                              │
│  ────●──────────────●──────────────●────                               │
│      │              │              │                                    │
│  ┌─────┐        ┌─────┐        ┌─────┐                                 │
│  │ img │        │ img │        │ img │   ← Timeline metaphor           │
│  └─────┘        └─────┘        └─────┘                                 │
│  label          label          label                                    │
│  📝 Narrative summary of the flow...                                    │
│                                                                         │
└─────────────────────────────────────────────────────────────────────────┘
```

---

## Ratings

| Option | Arrows | Captions | Compact | Scannable | GitHub MD | Rating |
|--------|:------:|:--------:|:-------:|:---------:|:---------:|:------:|
| **6. Horizontal + Captions** | ✅ | ✅ | ✅ | ✅ | ✅ | ⭐⭐⭐⭐⭐ |
| 7. Stacked Cards | ✅ | ✅✅ | ❌ | ⚠️ | ✅ | ⭐⭐⭐ |
| **8. Comic Strip + Legend** | ✅ | ✅✅ | ✅ | ✅ | ✅ | ⭐⭐⭐⭐⭐ |
| 9. Before/After | ✅ | ✅ | ✅ | ✅ | ✅ | ⭐⭐⭐⭐ |
| **10. Timeline Strip** | ✅ | ✅ | ✅ | ✅ | ✅ | ⭐⭐⭐⭐⭐ |

---

## Top 3 Recommendations

### 🥇 Option 8: Comic Strip + Legend

**Best for:** Maximum clarity with clean separation of visuals and explanation.

```markdown
| ① | → | ② | → | ③ |
|:---:|:---:|:---:|:---:|:---:|
| <img src="1-initial.png" width="180"/> | | <img src="2-filled.png" width="180"/> | | <img src="3-result.png" width="180"/> |

| Step | What Happened |
|:----:|---------------|
| ① | Redirected to `/Account/Login` after accessing protected route |
| ② | Entered test credentials: `admin@test.com` |
| ③ | Login failed: Invalid credentials |
```

**Why:** 
- Images tell the visual story (scan quickly)
- Legend provides the detailed narrative (read when needed)
- Numbered badges connect the two
- Clean, professional, documentation-friendly

---

### 🥈 Option 6: Horizontal Flow + Captions Below

**Best for:** Compact display with all info visible.

```markdown
| | | | | |
|:---:|:---:|:---:|:---:|:---:|
| <img src="1-initial.png" width="180"/> | → | <img src="2-filled.png" width="180"/> | → | <img src="3-result.png" width="180"/> |
| **Login Form** | | **Filled** | | **Result** |
| *Redirected* | | *admin@test.com* | | *Failed* |
```

**Why:**
- Everything visible at once
- Arrows clearly show flow
- Two-level captions (title + detail)
- Most compact of the detailed options

---

### 🥉 Option 10: Timeline Strip

**Best for:** Professional documentation with narrative summary.

```markdown
| Step 1 | | Step 2 | | Step 3 |
|:------:|:-:|:------:|:-:|:------:|
| <img src="1-initial.png" width="180"/> | ● | <img src="2-filled.png" width="180"/> | ● | <img src="3-result.png" width="180"/> |
| **Redirect** | ─ | **Fill** | ─ | **Result** |

> 📝 User accessed `/auth` → redirected to login → entered credentials → login failed
```

**Why:**
- Timeline metaphor is intuitive
- Summary narrative gives context
- Works well in technical docs

---

## Decision Points

### 1. Which layout style?

| Style | Best For |
|-------|----------|
| **Option 8 (Comic Strip)** | Maximum clarity, detailed explanations |
| **Option 6 (Horizontal)** | Compact, all-in-one view |
| **Option 10 (Timeline)** | Professional docs, narrative focus |

### 2. How much caption detail?

| Level | Example |
|-------|---------|
| **Minimal** | "Login Form" / "Filled" / "Result" |
| **Medium** | "Redirected to login" / "admin@test.com" / "Failed" |
| **Detailed** | Full sentences explaining each step |

**Recommendation:** Medium — enough to understand, not overwhelming.

### 3. Include summary narrative?

Options 8 and 10 include a summary line explaining the full flow.

**Recommendation:** Yes — helps users who just want to know "what happened" without examining each image.

---

## Next Steps

| Action | Owner | Priority |
|--------|-------|----------|
| CTO picks layout (6, 8, or 10) | [CTO] | P1 |
| CTO picks caption detail level | [CTO] | P1 |
| Implement in WorkspaceReporter | [Backend] | P1 |

---

*Created: 2025-12-31*  
*Previous: `105_cto-brief_screenshot-gallery-redesign.md`*
