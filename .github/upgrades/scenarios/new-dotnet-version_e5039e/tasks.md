# DotNetAvroSerializer .NET 10 Upgrade Tasks

## Overview

This document tracks the execution of DotNetAvroSerializer upgrade from .NET 8/9 to .NET 10 LTS. All components will be upgraded simultaneously in a single atomic operation, followed by testing and validation.

**Progress**: 0/4 tasks complete (0%) ![0%](https://progress-bar.xyz/0)

---

## Tasks

### [▶] TASK-001: Verify prerequisites
**References**: Plan §Migration Strategy

- [▶] (1) Verify .NET 10 SDK installed and available
- [ ] (2) .NET 10 SDK version meets minimum requirements (**Verify**)
- [ ] (3) Check global.json compatibility with .NET 10 (if file exists)
- [ ] (4) global.json compatible or updated for .NET 10 (**Verify**)

---

### [ ] TASK-002: Atomic framework and package upgrade
**References**: Plan §Migration Strategy, Plan §Project-by-Project Plans, Plan §Package Update Reference

- [ ] (1) Update DotNetAvroSerializer.Benchmarks: net9.0 → net10.0
- [ ] (2) Update DotNetAvroSerializer.Generators.Tests: net9.0 → net10.0
- [ ] (3) Update DotNetAvroSerializer.Write.Tests: net9.0 → net10.0
- [ ] (4) Update DotNetAvroSerializer: add net10.0 to existing net8.0;net9.0 multi-targeting
- [ ] (5) Confirm DotNetAvroSerializer.Generators remains on netstandard2.0 (no change)
- [ ] (6) All project TargetFramework properties updated correctly (**Verify**)
- [ ] (7) Update Newtonsoft.Json to version 13.0.4 in DotNetAvroSerializer.Generators project
- [ ] (8) Package reference updated (**Verify**)
- [ ] (9) Restore all dependencies for entire solution
- [ ] (10) All dependencies restored successfully (**Verify**)
- [ ] (11) Build entire solution and fix any compilation errors per Plan §Breaking Changes Catalog
- [ ] (12) Solution builds with 0 errors (**Verify**)

---

### [ ] TASK-003: Run full test suite and validate upgrade
**References**: Plan §Testing & Validation Strategy, Plan §Project-by-Project Plans

- [ ] (1) Run tests in DotNetAvroSerializer.Generators.Tests project
- [ ] (2) Run tests in DotNetAvroSerializer.Write.Tests project
- [ ] (3) Fix any test failures (reference Plan §Breaking Changes Catalog for guidance)
- [ ] (4) Re-run tests after fixes
- [ ] (5) All tests pass with 0 failures (**Verify**)

---

### [ ] TASK-004: Final commit
**References**: Plan §Source Control Strategy

- [ ] (1) Commit all changes with message: "Complete upgrade to .NET 10 LTS"

---
