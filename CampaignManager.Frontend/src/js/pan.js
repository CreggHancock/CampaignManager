import panzoom from "panzoom/dist/panzoom";

var panZoomTiger;
var blockingElements = [];

export function getPanOffset() {
  const board = document.getElementById("battleMap");
  var matrix = new DOMMatrix(window.getComputedStyle(board).transform);
  return { xOffset: matrix.e, yOffset: matrix.f, scale: matrix.a };
}

export function setDraggable(canDrag, blockingId) {
  console.log(`setting draggable to ${canDrag} for blocking id ${blockingId}`);
  var elementBlocking = blockingElements.some(e => e === blockingId);
  if (!canDrag && !elementBlocking) {
    blockingElements.push(blockingId);
    panZoomTiger.pause();
  } else if (canDrag && elementBlocking) {
    blockingElements = blockingElements.filter(e => e !== blockingId);
    if (blockingElements.length === 0) {
        panZoomTiger.resume();
    }
  }
  console.log(`Blocking elements after changes are ${blockingElements}`);
}

export function initialize() {
  window.requestAnimationFrame(() => {
    window.requestAnimationFrame(() => {
      const board = document.getElementById("battleMap");

      // Move the map
      if (!panZoomTiger) {
        panZoomTiger = panzoom(board, {
          maxZoom: 2, // adjust as needed
          minZoom: 0.5, // adjust as needed
          smoothScroll: true,
          filterKey: function (e, x, y, z) {
            // Don't zoom with ctrl + mouse wheel
            var shouldIgnore = e.ctrlKey || e.metaKey || e.altKey;
            return !shouldIgnore;
          },
          onTouch: () => blockingElements.length > 0,
        });
      }

    board.addEventListener("mouseenter", function (_) {
        if (panZoomTiger.isPaused()) {
            panZoomTiger.resume();
        }
    });

    board.addEventListener("mouseleave", function (_) {
        if (!panZoomTiger.isPaused()) {
            panZoomTiger.pause();
        }
    });

    board.addEventListener("touchstart", function (e) {
        if (e.target.closest(".map-token")) {
            blockingElements.push("touch-token");
            panZoomTiger.pause();
        }
    });

    board.addEventListener("touchend", function (_) {
        if (panZoomTiger.isPaused()) {
            blockingElements.push("touch-token");
            panZoomTiger.resume();
        }
    });

      // Listen for mouse move on the board
      board.addEventListener("mousemove", function (e) {
        if (e.buttons === 1 && !panZoomTiger.isPaused()) {
          // left mouse button pressed
          panZoomTiger.moveBy(e.movementX, e.movementY);
        }
      });
    });
  });
}
