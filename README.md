## 🚛 Latest Cargo Tool Updates

The GLS Companion Cargo Tool has received a major set of live updates based on real-world hauling tests and community feedback.

**No new GLS Companion download or reinstall is required.** Cargo Tool improvements are delivered live and will automatically be available the next time the tool is loaded.

### Cargo Grid & Physical Load Planning

The Cargo Tool now goes beyond route optimization and helps plan how cargo should actually be loaded into your selected ship.

- Uses ship-specific cargo grid geometry and cargo capacity
- Automatically detects supported maximum container sizes by ship
- Supports manual container-size override when needed
- Visually maps containers onto the ship's cargo grids
- Plans cargo placement based on physical cargo access
- Keeps cargo positioned toward the appropriate ramp or loading access
- Provides guided Load Sequence and Unload Mode views
- Identifies commodities, container sizes, quantities, destinations, and physical grid locations
- Highlights the cargo for the current unload stop while dimming cargo for later stops

### Intelligent Multi-Contract Planning

Multiple hauling contracts can now be evaluated together instead of treating every contract as an isolated run.

GLS Companion will:

- Combine compatible contracts when they fit within the selected ship
- Use available cargo space to begin oversized contracts when possible
- Carry unfinished containers forward into additional physical load sessions
- Track which containers have already been scheduled
- Preserve remaining contract cargo for subsequent runs
- Prevent oversized contracts from unnecessarily blocking smaller compatible contracts
- Build each physical load around the actual cargo remaining

This means a large contract does not automatically require waiting for an empty ship. If there is usable space remaining during another run, GLS Companion can begin moving that contract immediately.

### Continue Contracts Across Multiple Runs

If the selected ship cannot complete all remaining cargo in one trip, the Cargo Tool can now continue the contract across multiple physical loads.

After completing the current load, you can:

**Continue Contract with Current Ship**

Keep using the same ship and automatically generate the next load using only the remaining containers. This can be repeated until the contract is complete.

**Switch to a Different Ship**

GLS Companion can also identify ships capable of completing the remaining cargo in a single trip.

When another ship is selected, the tool automatically:

- Carries forward only the remaining contract cargo
- Rebuilds the physical cargo grid for the new ship
- Recalculates container compatibility
- Generates a new Load Sequence
- Generates a new Unload Mode plan
- Updates ship capacity and container-size validation
- Removes previous ship compatibility warnings when the newly selected ship resolves them

This allows a hauling session to begin with one ship and continue with a more capable ship without rebuilding the remaining contract manually.

### Ship & Container Compatibility

Cargo capacity alone does not determine whether a ship can complete a contract.

GLS Companion now evaluates the physical container sizes supported by the selected ship.

If a contract contains containers that are too large for that ship:

- The runner is warned before attempting the load
- Compatible containers can still be included in the current physical plan
- Incompatible containers remain tracked as unfinished cargo
- Another compatible ship can be selected later to finish the contract

This allows the planner to distinguish between **available SCU capacity** and **physical container compatibility**.

### Commodity-Aware Loading & Unloading

Load and unload instructions now identify exactly what each container contains.

Instead of displaying only:

`2 × 16 SCU`

the tool can display:

`2 × 16 SCU — Aluminum`

This information is included throughout the Load Sequence and Unload Mode views, making it significantly easier to verify cargo while working at freight elevators.

### Improved Cargo Grid Visualization

Cargo-grid rendering has also been improved across the supported ship library.

- Small cargo grids are automatically enlarged for readability
- Grid layouts remain centered within the visualization
- Large cargo layouts retain an appropriate scale
- Container placement remains visible throughout guided unloading
- Empty destinations are removed from the physical Load Sequence
- Destination-aware packing prevents later dropoffs from being accidentally blocked by earlier cargo

### Still Early Beta

GLS Companion remains in **Early Public Beta**, and continued testing is extremely valuable.

Cargo hauling can involve unusual ship layouts, cargo access points, container restrictions, contract combinations, and edge cases that are difficult to reproduce without actually running contracts in-game.

If you find a ship, contract, cargo grid, container size, route, or loading situation that does not behave correctly, please let us know.

Every report helps make GLS Companion better for the Star Citizen hauling community.

**United We Explore. Stronger We Thrive.**
