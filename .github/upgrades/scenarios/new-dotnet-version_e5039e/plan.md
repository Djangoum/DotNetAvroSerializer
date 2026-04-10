# .NET 10 Upgrade Plan - DotNetAvroSerializer

## Table of Contents

- [Executive Summary](#executive-summary)
- [Migration Strategy](#migration-strategy)
- [Detailed Dependency Analysis](#detailed-dependency-analysis)
- [Project-by-Project Plans](#project-by-project-plans)
  - [DotNetAvroSerializer.Generators](#dotnetavroserializergenerators)
  - [DotNetAvroSerializer](#dotnetavroserializer)
  - [DotNetAvroSerializer.Benchmarks](#dotnetavroserializerbenchmarks)
  - [DotNetAvroSerializer.Generators.Tests](#dotnetavroserializergeneratorstests)
  - [DotNetAvroSerializer.Write.Tests](#dotnetavroserializerwritetests)
- [Package Update Reference](#package-update-reference)
- [Breaking Changes Catalog](#breaking-changes-catalog)
- [Risk Management](#risk-management)
- [Testing & Validation Strategy](#testing--validation-strategy)
- [Complexity & Effort Assessment](#complexity--effort-assessment)
- [Source Control Strategy](#source-control-strategy)
- [Success Criteria](#success-criteria)

---

## Executive Summary

### Scenario Description

This plan guides the upgrade of DotNetAvroSerializer, a high-performance Avro serializer for .NET based on source generators, from its current state (.NET 8/9 with some .NET Standard 2.0 components) to .NET 10 LTS.

### Scope

**Projects Affected:** 5 projects
- 1 Source Generator Library (netstandard2.0) - **stays on netstandard2.0**
- 1 Runtime Library (net8.0;net9.0) - **adds net10.0 to multi-targeting**
- 1 Benchmark Project (net9.0) - **upgrades to net10.0**
- 2 Test Projects (net9.0) - **upgrade to net10.0**

**Current State:**
- Total LOC: 4,533
- Total Files: 89
- 14 NuGet packages (12 compatible, 1 upgrade recommended, 1 deprecated)
- All projects are SDK-style
- Simple dependency structure (depth: 2 levels)

### Target State

**Target Framework:** .NET 10 (LTS - Long Term Support)

**Expected Changes:**
- 3 projects: net9.0 → net10.0 (simple upgrade)
- 1 project: net8.0;net9.0 → net8.0;net9.0;net10.0 (add to multi-targeting)
- 1 project: netstandard2.0 → **remains netstandard2.0** (source generator best practice)
- 1 package upgrade: Newtonsoft.Json 13.0.* → 13.0.4
- 1 package deprecation noted: xunit 2.9.3 (addressed as optional improvement)

### Discovered Metrics

| Metric | Value | Classification |
|--------|-------|----------------|
| Total Projects | 5 | Small solution |
| Dependency Depth | 2 levels | Simple structure |
| Total LOC | 4,533 | Small codebase |
| High-Risk Projects | 0 | Low risk |
| Security Vulnerabilities | 0 | No critical issues |
| API Breaking Changes | 0 | Clean upgrade |
| Package Incompatibilities | 0 | All compatible |

### Complexity Classification

**Simple Solution** ✅

**Justification:**
- ≤5 projects (exactly 5)
- Dependency depth ≤2 (exactly 2)
- No high-risk indicators (no security issues, no API breaks)
- No incompatible packages
- All projects on modern .NET (8+) or netstandard2.0
- Well-maintained dependency structure with no circular dependencies

### Selected Strategy

**All-At-Once Strategy**

**Rationale:**
- Small solution size enables atomic upgrade
- Simple dependency structure reduces coordination complexity
- All projects already on modern .NET (no legacy Framework migrations)
- Excellent package compatibility (85.7% fully compatible)
- No breaking API changes detected
- Low risk profile allows for simultaneous update
- Source generator best practices well-understood (keep on netstandard2.0)

### Critical Issues

**None identified.** This is a clean upgrade with:
- ✅ No security vulnerabilities
- ✅ No blocking incompatibilities
- ✅ No API breaking changes
- ✅ All packages compatible or have clear upgrade paths

**Optional Improvements:**
- Consider migrating from deprecated xunit 2.9.3 to newer test framework (can be deferred)
- Apply recommended Newtonsoft.Json security/bug fix update

### Recommended Approach

Execute all project upgrades, package updates, and validation in a single coordinated operation. The solution's small size and clean dependency structure make this the most efficient approach.

### Iteration Strategy

Given the simple classification, this plan will use a **fast batch approach**:
- **Phase 1:** Discovery & Classification (complete)
- **Phase 2:** Foundation (dependency analysis, strategy, project stubs)
- **Phase 3:** Single consolidated detail iteration (all projects batched together)

**Expected Total Iterations:** 6-7 iterations

---

## Migration Strategy

### Approach Selection

**Selected: All-At-Once Strategy**

All projects in the solution are upgraded simultaneously in a single coordinated operation. All project files are updated to their target framework versions at once, creating a unified upgrade without intermediate states.

### Justification

#### Why All-At-Once Is Optimal Here

**Solution Characteristics:**
- ✅ Small solution (5 projects - well under 30 project threshold)
- ✅ Simple dependency structure (2-level depth, no cycles)
- ✅ Homogeneous codebase (all SDK-style, modern .NET)
- ✅ Low external dependency complexity (14 packages, 85.7% compatible)
- ✅ All projects on .NET 6+ or netstandard2.0
- ✅ Comprehensive test coverage (2 test projects)
- ✅ No breaking changes detected in assessment

**Risk Assessment:**
- Low initial risk (no API breaks, no incompatibilities)
- Small testing surface (4,533 LOC total)
- Team can coordinate single deployment
- No mission-critical constraints requiring zero downtime

#### Advantages for This Solution

1. **Fastest Completion:** Single pass through all projects
2. **No Multi-Targeting Complexity:** No temporary intermediate states needed
3. **All Projects Benefit Simultaneously:** .NET 10 LTS features available everywhere at once
4. **Clean Dependency Resolution:** No version conflicts from mixed framework targeting
5. **Simple Coordination:** One build, one test pass, one commit

#### Challenges and Mitigations

| Challenge | Mitigation |
|-----------|------------|
| Higher initial risk | Low actual risk due to clean assessment, comprehensive tests |
| Larger testing surface | Only 4,533 LOC, well-covered by existing tests |
| All developers adapt simultaneously | Small team, modern .NET knowledge already present |
| Requires coordinated deployment | Git branch workflow ensures clean rollback if needed |

### All-At-Once Strategy Rationale

**Atomic Operation Benefits:**
- All target framework properties updated together
- All package references updated together
- Single restore and build identifies all issues at once
- Single test pass validates entire upgrade
- One commit captures complete upgrade

**No Intermediate States:**
- No projects temporarily multi-targeting during transition
- No mixed framework versions causing confusion
- No staged rollout complexity
- Clean before/after comparison

### Dependency-Based Ordering

While all updates happen atomically, the logical execution flow respects dependencies:

**Phase 1: Core Libraries (Simultaneous Update)**
1. `DotNetAvroSerializer.Generators` - Remains netstandard2.0, update packages only
2. `DotNetAvroSerializer` - Add net10.0 to existing net8.0;net9.0 multi-targeting

**Phase 2: Consuming Projects (Simultaneous Update)**
3. `DotNetAvroSerializer.Benchmarks` - net9.0 → net10.0
4. `DotNetAvroSerializer.Generators.Tests` - net9.0 → net10.0
5. `DotNetAvroSerializer.Write.Tests` - net9.0 → net10.0

**Phase 3: Validation (Single Pass)**
6. Build entire solution
7. Fix any compilation errors (expected: minimal to none)
8. Run all tests
9. Verify benchmarks execute successfully

### Parallel vs Sequential Execution

**Simultaneous Execution** for all project file updates and package updates.

**Sequential Validation** for build and test:
1. Build all projects (one `dotnet build` command at solution level)
2. Run all tests (one `dotnet test` command at solution level)

**Rationale:** 
- Updates can happen simultaneously (they're independent file edits)
- Build must happen after all updates complete (dependency on updated files)
- Tests must happen after successful build (dependency on compiled assemblies)

### Multi-Targeting Approach

**DotNetAvroSerializer Runtime Library:**

**Current:** `<TargetFrameworks>net8.0;net9.0</TargetFrameworks>`

**Target:** `<TargetFrameworks>net8.0;net9.0;net10.0</TargetFrameworks>`

**Rationale:**
- Maintain backward compatibility with .NET 8 (LTS) consumers
- Keep .NET 9 (STS) support for current adopters
- Add .NET 10 (LTS) for new LTS adoption
- Allows library consumers to choose their preferred framework
- Minimal overhead (same codebase compiles for all three)

**All Other Projects:**

Single-targeting to net10.0 (or remaining on netstandard2.0 for generator).

**Rationale:**
- Tests and benchmarks only need to target latest framework
- Simplifies maintenance (no conditional compilation needed)
- Tests validate functionality works on net10.0, multi-targeting handled by library

---

## Detailed Dependency Analysis

### Dependency Graph Summary

The solution has a clean, two-level dependency structure:

```
Level 0 (Leaf - No Dependencies):
  - DotNetAvroSerializer.Generators (netstandard2.0)
  - DotNetAvroSerializer (net8.0;net9.0)

Level 1 (Depends on Level 0):
  - DotNetAvroSerializer.Benchmarks (net9.0)
    └─> DotNetAvroSerializer.Generators
    └─> DotNetAvroSerializer
  - DotNetAvroSerializer.Write.Tests (net9.0)
    └─> DotNetAvroSerializer.Generators
    └─> DotNetAvroSerializer
  - DotNetAvroSerializer.Generators.Tests (net9.0)
    └─> DotNetAvroSerializer.Generators
```

**Key Observations:**
- No circular dependencies
- Clear separation: source generator and runtime library are independent
- All dependent projects consume both the generator and runtime library (except generator tests)
- Maximum dependency depth: 1 level (very simple)

### Project Groupings by Migration Phase

Since we're using **All-At-Once Strategy**, all projects are upgraded simultaneously. However, they're logically grouped by role:

**Group 1: Core Libraries (Leaf Nodes - No Project Dependencies)**
1. `DotNetAvroSerializer.Generators` (netstandard2.0) - **No change**
2. `DotNetAvroSerializer` (net8.0;net9.0) - **Add net10.0**

**Group 2: Consuming Applications & Tests (Depend on Core)**
3. `DotNetAvroSerializer.Benchmarks` (net9.0) - **Upgrade to net10.0**
4. `DotNetAvroSerializer.Generators.Tests` (net9.0) - **Upgrade to net10.0**
5. `DotNetAvroSerializer.Write.Tests` (net9.0) - **Upgrade to net10.0**

**Migration Order Rationale:**
While all projects are updated in a single atomic operation, the logical validation order respects dependencies:
1. Core libraries are updated first (even though generator stays on netstandard2.0)
2. Runtime library gets net10.0 added to its multi-targeting
3. Consuming projects then reference the newly multi-targeted libraries
4. Tests validate the entire stack works on net10.0

### Critical Path Identification

**Primary Path:** 
```
DotNetAvroSerializer.Generators (no change) + DotNetAvroSerializer (+net10.0)
  → DotNetAvroSerializer.Write.Tests (→net10.0)
    → Full integration validation
```

This path represents the core serialization functionality and its comprehensive test suite. Success on this path indicates the upgrade is sound.

**Secondary Paths:**
- Generator Tests: Validates source generator continues working
- Benchmarks: Validates performance characteristics on new framework

### Circular Dependency Details

**None.** The solution has a clean acyclic dependency graph.

### Source Generator Considerations

**Important:** The `DotNetAvroSerializer.Generators` project targets `netstandard2.0` and **should remain on netstandard2.0** even after this upgrade.

**Reasoning:**
- Source generators run during compilation, not at runtime
- netstandard2.0 provides maximum compatibility with all .NET SDK versions (.NET 6, 7, 8, 9, 10+)
- Upgrading generator to net10.0 would prevent use in projects targeting older frameworks
- Microsoft's official guidance recommends netstandard2.0 for analyzer/generator projects

**Impact:**
- Generator project: No target framework change
- Generator project packages: May still need updates for bug fixes/security (e.g., Newtonsoft.Json)
- Consuming projects: Can target net10.0 while using netstandard2.0 generator (fully supported)

---

## Project-by-Project Plans

### DotNetAvroSerializer.Generators

**Current State:** netstandard2.0, 1,998 LOC, 39 files  
**Target State:** netstandard2.0 (no framework change), package updates only  
**Risk Level:** Low  

[Details to be filled]

---

### DotNetAvroSerializer

**Current State:** net8.0;net9.0, 619 LOC, 27 files  
**Target State:** net8.0;net9.0;net10.0 (add net10.0 to multi-targeting)  
**Risk Level:** Low  

[Details to be filled]

---

### DotNetAvroSerializer.Benchmarks

**Current State:** net9.0, 214 LOC, 5 files  
**Target State:** net10.0  
**Risk Level:** Low  

[Details to be filled]

---

### DotNetAvroSerializer.Generators.Tests

**Current State:** net9.0, 114 LOC, 6 files  
**Target State:** net10.0  
**Risk Level:** Low  

[Details to be filled]

---

### DotNetAvroSerializer.Write.Tests

**Current State:** net9.0, 1,588 LOC, 16 files  
**Target State:** net10.0  
**Risk Level:** Low  

[Details to be filled]

---

## Package Update Reference

[To be filled]

---

## Breaking Changes Catalog

[To be filled]

---

## Risk Management

### High-Level Risk Assessment

**Overall Risk Level: LOW** ✅

This upgrade presents minimal risk due to:
- Clean assessment results (0 breaking changes, 0 security vulnerabilities)
- Small, well-structured codebase
- Comprehensive test coverage
- All packages compatible with target framework
- Modern .NET to modern .NET upgrade (no legacy Framework migration)

### Risk Breakdown by Category

| Risk Category | Level | Count | Description |
|---------------|-------|-------|-------------|
| API Breaking Changes | ✅ None | 0 | No breaking changes detected in 5,261 APIs analyzed |
| Package Incompatibilities | ✅ None | 0 | All packages compatible or have clear upgrade paths |
| Security Vulnerabilities | ✅ None | 0 | No CVEs or security issues in current packages |
| Deprecated Packages | ⚠️ Low | 1 | xunit 2.9.3 (optional to address now) |
| Compilation Issues | ✅ Low | 0 | No expected compilation errors based on assessment |
| Runtime Behavior Changes | ✅ Low | 0 | No behavioral changes detected |

### Project-Level Risk Assessment

| Project | Complexity | Risk Level | Primary Risk Factors |
|---------|------------|------------|---------------------|
| DotNetAvroSerializer.Generators | Low | 🟢 Low | No framework change; package update only |
| DotNetAvroSerializer | Low | 🟢 Low | Multi-targeting addition; minimal code impact |
| DotNetAvroSerializer.Benchmarks | Low | 🟢 Low | Simple upgrade; 214 LOC |
| DotNetAvroSerializer.Generators.Tests | Low | 🟢 Low | Simple upgrade; 114 LOC; deprecated package |
| DotNetAvroSerializer.Write.Tests | Low | 🟢 Low | Simple upgrade; 1,588 LOC; deprecated package |

### Specific Risks and Mitigations

#### Risk 1: Deprecated xunit Package

**Description:** xunit 2.9.3 is marked as deprecated in both test projects.

**Impact:** Low - Tests may continue working, but package is no longer maintained.

**Mitigation:**
- **Option A (Recommended for this upgrade):** Defer xunit migration to separate effort
  - Current xunit 2.9.3 is compatible with net10.0
  - Tests will continue to run successfully
  - Allows upgrade to complete quickly
  - Plan separate xunit migration task post-upgrade

- **Option B (Optional):** Address during this upgrade
  - Research xunit's recommended migration path
  - May require test code changes
  - Increases upgrade scope and timeline

**Recommendation:** Option A - defer to post-upgrade cleanup phase.

#### Risk 2: Source Generator Compatibility

**Description:** Source generator stays on netstandard2.0 while consuming projects upgrade to net10.0.

**Impact:** Very Low - This is the recommended configuration.

**Mitigation:**
- Verify generator functions correctly when referenced by net10.0 projects (expected: yes)
- Test generated code compiles on net10.0 (covered by existing tests)
- No action needed - this is best practice

#### Risk 3: Multi-Targeting Build Complexity

**Description:** DotNetAvroSerializer adds third target framework (net10.0).

**Impact:** Low - Minimal complexity increase.

**Mitigation:**
- Existing multi-targeting infrastructure already in place (net8.0;net9.0)
- Adding net10.0 follows established pattern
- Build process already handles multiple targets
- Test on net10.0 to ensure generated outputs are correct

### Contingency Plans

#### If Compilation Errors Occur

**Unlikely based on assessment, but if they happen:**

1. **Check for framework-specific API usage:**
   - Review compilation errors for API availability issues
   - Consult breaking changes documentation: https://learn.microsoft.com/en-us/dotnet/core/compatibility/
   - Apply conditional compilation if needed: `#if NET10_0_OR_GREATER`

2. **Package version conflicts:**
   - Verify all packages restored correctly (`dotnet restore --force`)
   - Check for transitive dependency conflicts
   - Update conflicting packages to compatible versions

#### If Tests Fail

**Expected: Tests should pass, but if failures occur:**

1. **Behavioral changes:**
   - Review .NET 10 behavioral changes documentation
   - Identify which behavior changed
   - Update test expectations or implementation code as needed

2. **Performance regressions:**
   - Run benchmarks to identify performance changes
   - Investigate if .NET 10 introduces optimization changes
   - Adjust code if necessary (unlikely)

#### If xunit Issues Arise

**If deprecated xunit causes problems:**

1. **Short-term fix:**
   - Lock xunit to 2.9.3 explicitly
   - Document technical debt
   - Continue with upgrade

2. **Medium-term solution:**
   - Plan xunit migration as follow-up task
   - Research recommended xunit upgrade path
   - Execute as separate PR after .NET 10 upgrade stabilizes

### Rollback Plan

**If critical issues are encountered:**

1. **Git branch isolation:** All work on `upgrade-to-NET10` branch
2. **Rollback command:** `git checkout main` (instant revert)
3. **No production impact:** Changes not merged until validated
4. **Retry approach:** Address issues individually, re-attempt upgrade

**Rollback triggers:**
- Compilation errors that cannot be resolved within reasonable time
- Critical test failures indicating functionality broken
- Performance degradation exceeding acceptable thresholds
- Unforeseen compatibility issues blocking development

### All-At-Once Strategy Risk Factors

**Specific risks when using atomic upgrade approach:**

| Risk Factor | Impact | Mitigation |
|-------------|--------|------------|
| All projects fail together | Low | Assessment shows clean compatibility; tests validate |
| Larger testing surface | Low | Only 4,533 LOC total; well-covered by tests |
| Coordination required | Low | Single developer workflow; Git branch isolation |
| Rollback affects everything | Low | Git branch makes rollback instant |

**Risk Tolerance:** Given the clean assessment and small solution size, All-At-Once risk is acceptable and preferred over incremental complexity.

---

## Testing & Validation Strategy

[To be filled]

---

## Complexity & Effort Assessment

### Overall Complexity: LOW

The solution's characteristics place it firmly in the "simple" category for .NET upgrades.

### Per-Project Complexity

| Project | Complexity | LOC | Files | Dependencies | Packages | Risk | Rationale |
|---------|------------|-----|-------|--------------|----------|------|-----------|
| DotNetAvroSerializer.Generators | **Low** | 1,998 | 39 | 0 | 6 | Low | No framework change; netstandard2.0 stays; 1 package update |
| DotNetAvroSerializer | **Low** | 619 | 27 | 0 | 0 | Low | Add target framework; no breaking changes |
| DotNetAvroSerializer.Benchmarks | **Low** | 214 | 5 | 2 | 2 | Low | Simple upgrade; minimal code; benchmarking only |
| DotNetAvroSerializer.Generators.Tests | **Low** | 114 | 6 | 1 | 6 | Low | Simple upgrade; small test suite; deprecated xunit (defer) |
| DotNetAvroSerializer.Write.Tests | **Low** | 1,588 | 16 | 2 | 6 | Low | Larger test suite; no breaking changes expected |

### Phase Complexity Assessment

Since we're using **All-At-Once Strategy**, there's effectively one atomic phase with logical sub-phases:

#### Phase 1: Atomic Upgrade Operation

**Sub-Phase 1a: Update Project Files**
- **Complexity:** Low
- **Scope:** 4 project file edits (1 stays unchanged)
  - 3 simple replacements: `net9.0` → `net10.0`
  - 1 multi-targeting addition: `net8.0;net9.0` → `net8.0;net9.0;net10.0`
- **Expected Issues:** None

**Sub-Phase 1b: Update Package References**
- **Complexity:** Low
- **Scope:** 1 package update across 1 project
  - Newtonsoft.Json: 13.0.* → 13.0.4 (in DotNetAvroSerializer.Generators)
- **Expected Issues:** None (minor version update)

**Sub-Phase 1c: Restore and Build**
- **Complexity:** Low
- **Scope:** Restore all projects, build entire solution
- **Expected Issues:** None (assessment shows 0 API breaking changes)

**Sub-Phase 1d: Fix Compilation Errors (if any)**
- **Complexity:** Low
- **Scope:** Expected 0 errors based on assessment
- **Expected Issues:** None

**Sub-Phase 1e: Test Validation**
- **Complexity:** Low
- **Scope:** Run 2 test projects (114 + 1,588 LOC)
- **Expected Issues:** None (0 behavioral changes detected)

#### Phase 2: Optional Improvements (Deferred)

**xunit Migration**
- **Complexity:** Medium (out of scope for this upgrade)
- **Recommendation:** Separate effort post-upgrade

### Dependency Ordering Complexity

**Complexity: Very Low**

- **Depth:** 2 levels (leaf nodes → consuming projects)
- **Cycles:** None (acyclic graph)
- **Conflicts:** None (clean dependency resolution)

**Ordering:**
1. Core libraries (no dependencies) - Updated first
2. Consuming projects (depend on core) - Updated second
3. Validation - Single pass

No complicated sequencing required; All-At-Once handles dependencies naturally.

### Resource Requirements

#### Skills Required

| Skill | Level | Rationale |
|-------|-------|-----------|
| .NET Development | Intermediate | Understanding of target frameworks, multi-targeting |
| Source Generators | Intermediate | Knowing to keep generator on netstandard2.0 |
| Package Management | Basic | Simple package updates via project file edits |
| Testing | Basic | Running existing tests, interpreting results |
| Git | Basic | Branch management, commit, potential rollback |

#### Parallel Execution Capacity

**Not Applicable** - All-At-Once strategy performs atomic update.

**Validation can be parallelized:**
- Build: Single command at solution level
- Tests: Can run test projects in parallel (handled by test runner automatically)

#### Time Considerations

**IMPORTANT:** No specific time estimates provided (agent cannot reliably predict duration).

**Relative Effort:**
- **Low complexity overall** due to clean assessment
- **Small codebase** (4,533 LOC) reduces investigation time
- **Comprehensive tests** enable quick validation
- **No breaking changes** eliminate debugging/fixing time
- **Simple dependency structure** eliminates coordination overhead

**Expected Effort Indicators:**
- Manual work: Project file edits (minutes)
- Automated work: Build, test (minutes)
- Verification: Review, validation (minutes to hours based on thoroughness)

### Complexity Factors Summary

| Factor | Level | Impact on Effort |
|--------|-------|------------------|
| Solution Size | Small (5 projects) | Reduces effort significantly |
| Codebase Size | Small (4,533 LOC) | Minimal investigation needed |
| Dependency Structure | Simple (2 levels, acyclic) | No coordination complexity |
| API Breaking Changes | None (0 detected) | No debugging/fixing needed |
| Package Issues | Minimal (1 update) | Trivial resolution |
| Test Coverage | Good (2 test projects) | Faster validation |
| Project Types | Homogeneous (all SDK-style) | Consistent approach |

**Overall Assessment:** This is a **textbook simple upgrade** with exceptionally favorable characteristics.

---

## Source Control Strategy

[To be filled]

---

## Success Criteria

[To be filled]
