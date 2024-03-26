import panzoom from "panzoom/dist/panzoom";

let draggable = true;

export function getPanOffset() {
    const board = document.getElementById("battleMap");
    var matrix = new DOMMatrix(window.getComputedStyle(board).transform)
    return { xOffset: matrix.e, yOffset: matrix.f, scale: matrix.a };
}

export function setDraggable(canDrag) {
    draggable = canDrag;
}

export function initialize() {
  window.requestAnimationFrame(() => {
    console.log("initializing pan");
    const board = document.getElementById("battleMap");

    // Move the map
    const panZoomTiger = panzoom(board, {
      maxZoom: 2, // adjust as needed
      minZoom: 0.5, // adjust as needed
      smoothScroll: true,
      filterKey: function (e, x, y, z) {
        // Don't zoom with ctrl + mouse wheel
        var shouldIgnore = e.ctrlKey || e.metaKey || e.altKey;
        return !shouldIgnore;
      },
    });

    board.addEventListener("mouseenter", function (_) {
        if (panZoomTiger.isPaused()) {
            panZoomTiger.resume();
        }
    })

    board.addEventListener("mouseleave", function (_) {
        if (!panZoomTiger.isPaused()) {
            panZoomTiger.pause();
        }
    })

    // Listen for mouse move on the board
    board.addEventListener("mousemove", function (e) {
      if (e.buttons === 1 && draggable) {
        // left mouse button pressed
        panZoomTiger.moveBy(e.movementX, e.movementY);
        console.log("moving mouse");
      }
    });
  });
}
