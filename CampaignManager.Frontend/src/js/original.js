let initiativeCards = Array.from(document.querySelectorAll('#initiativeRotation .initiative-card'));
let mapTokens = Array.from(document.querySelectorAll('#battleMap .initiative-card'));
let originalOrder = [...initiativeCards];
const initiativeContainer = document.getElementById('initiativeRotation');
const zoomStep = 1.25;
const board = document.getElementById('battleMap');

let isHovering = false;
let initialScrollPosition = 0;

function scrollToActive() {
    const activeCard = document.querySelector('#initiativeRotation .initiative-card.active');
    const initiativeContainer = document.getElementById('initiativeRotation');

    if (activeCard) {
        const cardBounds = activeCard.getBoundingClientRect();
        const containerBounds = initiativeContainer.getBoundingClientRect();

        if (activeCard === initiativeCards[0]) {
            initiativeContainer.scrollTo({
                left: 0,
                behavior: 'smooth'
            });
        } else if (cardBounds.right > containerBounds.right) {
            const difference = cardBounds.right - containerBounds.right;
            const currentScroll = initiativeContainer.scrollLeft;

            initiativeContainer.scrollTo({
                left: currentScroll + difference + 10,
                behavior: 'smooth'
            });
        }
    }
}
// calculate & add dexterity modifier
function getDexModifier(dexValue) {
    return Math.floor(dexValue / 2) - 5;
}
function updateDexModAttributes() {
    initiativeCards.forEach(card => {
        // Skip cards that already have a dex-mod attribute updated
        if (!card.hasAttribute('data-dex-mod-updated')) {
            const dexValue = parseInt(card.getAttribute('data-dex'), 10);
            const dexMod = getDexModifier(dexValue);
            card.setAttribute('data-dex-mod', dexMod);
            card.setAttribute('data-dex-mod-updated', 'true'); // Mark it as updated
        }
    });
}
updateDexModAttributes();

// Add player input boxes
function addInputForPlayers() {
    const playerCards = document.querySelectorAll('.initiative-card:not(.enemy):not([data-input-added])');

    playerCards.forEach(card => {
      // Create the input element
      const input = document.createElement('input');
      input.type = 'text';
      input.className = 'initiative-input';
      input.id = card.id + '-input';

      card.appendChild(input);
      card.setAttribute('data-input-added', 'true');

      const characterName = card.dataset.name;
      // Create the new card div
      const nameCard = document.createElement('span');
      nameCard.classList.add('character-name');
      nameCard.innerHTML += characterName;
      card.appendChild(nameCard);
    });
}
addInputForPlayers();


// Add character names
function addNamesToTokens() {
    const characterCards = document.querySelectorAll('.initiative-card:not([data-input-added])');
    characterCards.forEach(card => {
      const characterName = card.dataset.name;
      // Create the new card div
      const nameCard = document.createElement('span');
      nameCard.classList.add('character-name');
      nameCard.innerHTML += characterName;
      card.appendChild(nameCard);
    });
}
addNamesToTokens();


// Roll initiative and store the results
function rollD20() {
  return Math.floor(Math.random() * 20) + 1;
}
document.getElementById('rollInitiative').addEventListener('click', function() {
    // Array to store rolled initiatives along with the card reference
    const rollsWithCards = [];
    
    initiativeCards.forEach(card => {
        // Check for manual input in the initiative-input of the card
        const manualInput = card.querySelector('.initiative-input');
        let roll;
        
        if (manualInput && manualInput.value) {
            roll = parseInt(manualInput.value, 10);
        } else {
            roll = rollD20() + parseInt(card.getAttribute('data-dex-mod'), 10);
        }

        card.setAttribute('data-initiative', roll);
        
        // Store the rolled value along with the card reference
        rollsWithCards.push({ roll, card });
        
        let rollSpan = card.querySelector('.roll-value');
        if (!rollSpan) {
            rollSpan = document.createElement('span');
            rollSpan.classList.add('roll-value');
            
            const randomDelay = Math.random();
            rollSpan.style.animation = `slideIntro 0.5s forwards ${randomDelay}s`;
            
            card.appendChild(rollSpan);
        }
        
        rollSpan.textContent = roll;
        const img = card.querySelector('img');
        card.insertBefore(rollSpan, img.nextSibling);
    });

    // Sort the rollsWithCards array in descending order
    rollsWithCards.sort((a, b) => b.roll - a.roll);

    // Get top 3 and lowest 3 rolled values
    const top3Rolls = rollsWithCards.slice(0, 2).map(item => item.roll);
    const low3Rolls = rollsWithCards.slice(-2).map(item => item.roll);

    // Assign classes to cards based on their rolled values
    rollsWithCards.forEach(item => {
        if (top3Rolls.includes(item.roll)) {
            item.card.classList.add('high-roll');
        }
        if (low3Rolls.includes(item.roll)) {
            item.card.classList.add('low-roll');
        }
    });

    // Hide the 'Roll Initiatives' button and Show the 'Start Combat' button
    document.getElementById('rollInitiative').classList.add('button-hidden');
    document.getElementById('addCharacter').classList.add('button-hidden');
    document.getElementById('startCombat').classList.remove('button-hidden');
    document.querySelectorAll('.hud-wrapper .initiative-input').forEach(element => {
        element.style.display = 'none';
    });
});
function setActive(card) {
    const initiativeCards = Array.from(document.querySelectorAll('#initiativeRotation .initiative-card'));
    // Remove 'active' from all initiative cards
    initiativeCards.forEach(c => c.classList.remove('active'));
    
    // Remove 'active' from all map tokens and set their pointer-events to none
    const allMapTokens = document.querySelectorAll(`#battleMap .initiative-card`);
    allMapTokens.forEach(token => {
      token.classList.remove('active');
      token.style.pointerEvents = 'none';
    });

    // Set the specified card and its corresponding token as 'active'
    card.classList.add('active');
    const mapToken = document.querySelector(`#battleMap #${card.id}`);
    if (mapToken) {
      mapToken.classList.add('active');
      mapToken.style.pointerEvents = 'auto';
    }

    centerOnActiveToken();
}

//Start Combat
document.getElementById('startCombat').addEventListener('click', function() {
    // Get the current positions of all the cards
    let initialPositions = {};
    initiativeCards.forEach(card => {
        initialPositions[card.id] = card.getBoundingClientRect().top;
    });
    // sort characters
    initiativeCards.sort((a, b) => {
        const rollA = parseInt(a.getAttribute('data-initiative'), 10);
        const rollB = parseInt(b.getAttribute('data-initiative'), 10);
        return rollB - rollA;  // Sort in descending order
    });
    const initiativeRotation = document.getElementById('initiativeRotation');
    initiativeCards.forEach(card => {
        initiativeRotation.appendChild(card);
        // Remove the span containing the rolled value
        const rollSpan = card.querySelector('.roll-value');
        if (rollSpan) {
            card.removeChild(rollSpan);
        }
    });
    // After sorting, calculate the difference between old and new positions
    initiativeCards.forEach(card => {
        const finalPosition = card.getBoundingClientRect().top;
        const moveOffset = initialPositions[card.id] - finalPosition;
        if (moveOffset) {
            card.style.transform = `translateY(${moveOffset}px)`;
        }
    });
    // Trigger a reflow, allowing the transform to take effect
    void document.body.offsetHeight;

    // Reset the transform to none, causing the transition to occur
    setTimeout(() => {
        initiativeCards.forEach(card => {
            card.style.transform = '';
        });
    }, 50);
    // make first character active
    setActive(initiativeCards[0]);
    // Remove 'active' class from other characters
    for(let i = 1; i < initiativeCards.length; i++) {
        initiativeCards[i].classList.remove('active');
    }
    // Add "introduce" class to the corresponding character-introduction element
    const activeCardId = initiativeCards[0].id;
    const correspondingIntroduction = document.querySelector(`.character-introduction[data-name="${activeCardId}"]`);
    if(correspondingIntroduction) {
        correspondingIntroduction.classList.add('introduce');
    }
    // Hide the 'Start Combat' button
    document.getElementById('startCombat').classList.add('button-hidden');
    document.getElementById('endTurn').classList.remove('button-hidden');
    document.getElementById('reset').classList.remove('button-hidden');
  
    mapTokens.forEach(token => token.style.pointerEvents = 'none');
});

//End Turn
document.getElementById('endTurn').addEventListener('click', function() {
    let activeIndex = -1;
    const totalCards = initiativeCards.length;
    const mapTokens = document.querySelectorAll(`#battleMap .initiative-card`);

    // Find the currently active character
    for(let i = 0; i < totalCards; i++) {
        if(initiativeCards[i].classList.contains('active')) {
            activeIndex = i;
            break;
        }
    }

    if (activeIndex !== -1) {
        // Determine the next character in order
        const nextIndex = (activeIndex + 1) % totalCards;

        // Hide the previous introduction
        const prevIntro = document.querySelector(`.character-introduction[data-name="${initiativeCards[activeIndex].id}"]`);
        if (prevIntro) prevIntro.classList.remove('introduce');

        // Add 'active' class to the next character
        setActive(initiativeCards[nextIndex]);
        
        // Show the next introduction
        const nextIntro = document.querySelector(`.character-introduction[data-name="${initiativeCards[nextIndex].id}"]`);
        if (nextIntro) nextIntro.classList.add('introduce');
    }
    // Scroll the active card into view
    scrollToActive();
});

//Reset
document.getElementById('reset').addEventListener('click', function() {
    // Remove 'active' class from all characters
    initiativeCards.forEach(card => {
        card.classList.remove('active');
        
        // Remove the span containing the roll value
        const spanElement = card.querySelector('.roll-value');
        if(spanElement) {
            card.removeChild(spanElement);
        }

        // Reset the input's value to an empty string
        const manualInput = card.querySelector('.initiative-input');
        if (manualInput) {
            manualInput.value = '';
        }
    });
    const allMapTokens = document.querySelectorAll('#battleMap .initiative-card');
    allMapTokens.forEach(token => {
        token.classList.remove('active');
        token.style.pointerEvents = 'auto';  // setting pointer events to none for all tokens
    });

    // Restore the original order of the cards
    const initiativeRotation = document.getElementById('initiativeRotation');
    originalOrder.forEach(card => {
        initiativeRotation.appendChild(card);
    });
    document.getElementById('rollInitiative').classList.remove('button-hidden');
    document.getElementById('addCharacter').classList.remove('button-hidden');
    document.getElementById('startCombat').classList.add('button-hidden');
    document.getElementById('endTurn').classList.add('button-hidden');
    document.getElementById('reset').classList.add('button-hidden');
    document.querySelectorAll('.hud-wrapper .initiative-input').forEach(element => {
        element.style.display = 'flex';
    });
});

// Move the map
const panZoomTiger = panzoom(board, {
  maxZoom: 2, // adjust as needed
  minZoom: 0.5, // adjust as needed
  smoothScroll: true,
  filterKey: function(e, x, y, z) {
    // Don't zoom with ctrl + mouse wheel
    var shouldIgnore = e.ctrlKey || e.metaKey || e.altKey;
    return !shouldIgnore;
  }
});


// Listen for mouse move on the board
board.addEventListener('mousemove', function(e) {
  if (e.buttons === 1) { // left mouse button pressed
    panZoomTiger.moveBy(e.movementX, e.movementY);
  }
});


// Update token positions on resize
function updateTokenPositions() {
  mapTokens.forEach(card => {
    const rect = board.getBoundingClientRect();
    const posX = rect.width * card.initialX;
    const posY = rect.height * card.initialY;

    card.style.left = `${posX}px`;
    card.style.top = `${posY}px`;
  });
}

// Add player tokens to map
function addTokensToMap() {
  const spacingX = board.clientWidth / 16;
  const spacingY = board.clientHeight / 16;

  initiativeCards.forEach((card, index) => {
      let clone = card.cloneNode(true);

      let column = index % 15;
      let row = Math.floor(index / 15);

      let posX = (column + 1) * spacingX;
      let posY = (row + 1) * spacingY;

      clone.style.position = "absolute";
      clone.style.left = `${posX}px`;
      clone.style.top = `${posY}px`;
      clone.style.transform = `translate(-50%, -50%)`;

      board.insertBefore(clone, board.firstChild);
      makeDraggable(clone);
  });
  updateTokenPositions();
}
addTokensToMap();



let currentScale = 1; // This will keep track of the current zoom level

// Assuming panZoomTiger is your Panzoom instance
panZoomTiger.on('transform', function(e) {
  // Access the scale from the transform data
  currentScale = e.getTransform().scale;
});

function makeDraggable(element) {
  let isDragging = false;
  let previousX;
  let previousY;

  element.addEventListener('mousedown', function(downEvent) {
    // Immediately stop the event from propagating to the map
    downEvent.stopPropagation();
    downEvent.preventDefault(); // Prevent default to avoid any browser-specific drag behavior

    // Mark as dragging
    isDragging = true;
    previousX = downEvent.clientX;
    previousY = downEvent.clientY;

    // Disable panzoom immediately to avoid moving the map
    panZoomTiger.pause();
  }, true); // Use capturing to ensure this runs before bubbling events

  document.addEventListener('mousemove', function(moveEvent) {
    if (!isDragging) return;

    moveEvent.stopPropagation(); // Stop mousemove from triggering panzoom

    const deltaX = (moveEvent.clientX - previousX) / currentScale;
    const deltaY = (moveEvent.clientY - previousY) / currentScale;

    previousX = moveEvent.clientX;
    previousY = moveEvent.clientY;

    const currentLeft = parseFloat(element.style.left) || 0;
    const currentTop = parseFloat(element.style.top) || 0;
    element.style.left = `${currentLeft + deltaX}px`;
    element.style.top = `${currentTop + deltaY}px`;
  }, true); // Use capturing to ensure this runs before bubbling events

  document.addEventListener('mouseup', function(upEvent) {
    if (isDragging) {
      isDragging = false;
      panZoomTiger.resume(); // Re-enable panzoom after dragging ends

      upEvent.stopPropagation(); // Stop mouseup from triggering any panzoom resume
    }
  }, true); // Use capturing to ensure this runs before bubbling events
}









//center on active player
function centerOnActiveToken() {
    const activeToken = document.querySelector('#battleMap .initiative-card.active');
    if (!activeToken) return;

    const tokenRect = activeToken.getBoundingClientRect();
    const battleMapRect = document.getElementById('battleMap').getBoundingClientRect();

    const tokenCenterX = tokenRect.left + tokenRect.width / 2 - battleMapRect.left;
    const tokenCenterY = tokenRect.top + tokenRect.height / 2 - battleMapRect.top;

    const viewportCenterX = window.innerWidth / 2;
    const viewportCenterY = window.innerHeight / 2;

    const transformData = panZoomTiger.getTransform();
    const scale = transformData.scale;

    const translateX = viewportCenterX / scale - tokenCenterX;
    const translateY = viewportCenterY / scale - tokenCenterY;

    const battleMap = document.getElementById('battleMap');

    battleMap.style.transition = 'transform 0.5s ease-out';

    panZoomTiger.moveTo(translateX, translateY);

    setTimeout(() => {
        battleMap.style.transition = '';
    }, 500);
}
// Function to center on a given token
function centerOnToken(token) {
    if (!token) return;

    const tokenRect = token.getBoundingClientRect();
    const battleMapRect = document.getElementById('battleMap').getBoundingClientRect();

    const tokenCenterX = tokenRect.left + tokenRect.width / 2 - battleMapRect.left;
    const tokenCenterY = tokenRect.top + tokenRect.height / 2 - battleMapRect.top;

    const viewportCenterX = window.innerWidth / 2;
    const viewportCenterY = window.innerHeight / 2;

    const transformData = panZoomTiger.getTransform();
    const scale = transformData.scale;

    const translateX = viewportCenterX / scale - tokenCenterX;
    const translateY = viewportCenterY / scale - tokenCenterY;

    const battleMap = document.getElementById('battleMap');

    battleMap.style.transition = 'transform 0.5s ease-out';

    panZoomTiger.moveTo(translateX, translateY);

    setTimeout(() => {
        battleMap.style.transition = '';
    }, 500);
}
// Event listeners for each initiative card
initiativeCards.forEach(card => {
    card.addEventListener('click', () => {
        // Find the corresponding mapToken using the card's ID
        const mapToken = document.querySelector(`#battleMap .initiative-card#${card.id}`);
        centerOnToken(mapToken); // Call the modified center function
    });
});


// add character skills and modifiers
function getStatModifier(value) {
    return Math.floor((value - 10) / 2);
}

function generateCharacterDetailsMarkup(card) {
    const attributes = ['dex'];
    const labels = ['Dex.'];

    let detailsMarkup = '<div class="character-details">';

    for (let i = 0; i < attributes.length; i++) {
        const value = parseInt(card.dataset[attributes[i]]);
            const modifier = getStatModifier(value);
            detailsMarkup += `
              <div class="stats ${attributes[i]}">
                <span class="stat-label">${labels[i]}</span>
                <span class="stat-mod">${modifier >= 0 ? '+' + modifier : modifier}</span>
                <span class="stat-score">(${value})</span>
              </div>`;
    }
    detailsMarkup += '</div>';
    return detailsMarkup;
}

document.addEventListener('DOMContentLoaded', function() {
    initiativeCards.forEach(card => {
        const detailsMarkup = generateCharacterDetailsMarkup(card);
        const imageElement = card.querySelector('img'); // Assuming you have an img element for the character's image
        if (imageElement) {
            imageElement.insertAdjacentHTML('afterend', detailsMarkup);
        }
    });
});


document.getElementById('addCharacter').addEventListener('click', function() {
  document.getElementById('createCharacterForm').classList.remove('element-hidden');
});
document.getElementById('cancelAddCharacter').addEventListener('click', function() {
  document.getElementById('createCharacterForm').classList.add('element-hidden');
});


function addToken(newCard) {
  const spacingX = board.clientWidth / 16;
  const spacingY = board.clientHeight / 16;

  const initiativeCards = Array.from(document.getElementsByClassName('initiative-card'));
  let index = initiativeCards.length - 1; // Position of the new card

  // Clone and create a unique ID for the token
  let clone = newCard.cloneNode(true);
  let tokenID = newCard.id; // This creates a unique ID for the token
  clone.id = tokenID;
  clone.classList.add('map-token');

  let column = index % 15;
  let row = Math.floor(index / 15);

  let posX = (column + 1) * spacingX;
  let posY = (row + 1) * spacingY;

  clone.style.position = "absolute";
  clone.style.left = `${posX}px`;
  clone.style.top = `${posY}px`;
  clone.style.transform = `translate(-50%, -50%)`;

  // Add the remove button to the clone for the map
  const removeButton = document.createElement('button');
  removeButton.type = 'button';
  removeButton.innerHTML = '<i class="fa-solid fa-skull"></i>';
  removeButton.classList.add('remove-button');
  // When adding the remove button to the token, update the click event
  removeButton.addEventListener('click', function() {
    removeCharacter(newCard.id, tokenID); // Pass both IDs
  });

  clone.appendChild(removeButton);

  // Assuming board is already defined and is the container for the tokens
  board.insertBefore(clone, board.firstChild); // Insert at the beginning of the game board
  makeDraggable(clone); // I assume this makes the token draggable

  // Now, push the clone to the mapTokens array to keep track of all tokens
  mapTokens.push(clone);
  updateTokenPositions(clone);
  addNamesToTokens(clone);
}

// Update the removeCharacter function to take two IDs
function removeCharacter(cardId, tokenId) {
  // Find and remove the initiative card
  const initiativeCard = document.getElementById(cardId);
  if (initiativeCard) {
    initiativeCard.remove();
    initiativeCards = initiativeCards.filter(card => card.id !== cardId);
  }

  // Find and remove the token from the map
  const mapToken = document.getElementById(tokenId);
  if (mapToken) {
    mapToken.remove();
    // Update your mapTokens array if you have one
  }
}


// Object to keep track of how many of each type have been added
let characterCounts = {
  player: 0,
  ally: 0,
  enemy: 0
};

document.getElementById('submitAddCharacter').addEventListener('click', function() {
  // Retrieve the input values
  const name = document.getElementById('characterName').value;
  const dexterity = document.getElementById('characterDex').value;
  const imageUrl = document.getElementById('characterImage').value;
  const characterType = document.getElementById('characterType').value;
  
  // Increment the appropriate count and create the ID
  characterCounts[characterType]++;
  const uniqueId = `${characterType}${characterCounts[characterType]}`;

  // Create the new card div
  const cardDiv = document.createElement('div');
  cardDiv.id = uniqueId; // Set the unique ID here
  cardDiv.classList.add('initiative-card', characterType);
  cardDiv.dataset.dex = dexterity;
  cardDiv.dataset.name = name;

  // Add an image to the card
  const image = document.createElement('img');
  image.src = imageUrl;
  image.title = name;
  cardDiv.appendChild(image);

  // Generate the character details markup and append it to the card
  const detailsMarkup = generateCharacterDetailsMarkup(cardDiv);
  cardDiv.innerHTML += detailsMarkup; // Append the details to the card's HTML
  
  // Now update your arrays
  initiativeCards.push(cardDiv);
  originalOrder.push(cardDiv);
  

  // Add the card to the initiativeRotation div
  document.getElementById('initiativeRotation').appendChild(cardDiv);
  
  // If character type is "enemy", add a remove button
  if (characterType === 'enemy' || 'ally') {
    const removeButton = document.createElement('button');
    removeButton.type = 'button';
    removeButton.innerHTML = '<i class="fa-solid fa-skull"></i>'; // Set inner HTML to include font awesome icon
    removeButton.id = `remove${uniqueId}`; // Unique ID using the character card ID
    removeButton.classList.add('remove-button'); // Optional: Add a class for styling
    // ... (other properties)
    let tokenID = 'token-' + uniqueId;
    removeButton.addEventListener('click', function() {
      removeCharacter(uniqueId, tokenID); // Pass both IDs
    });
    cardDiv.appendChild(removeButton);
  }


  // Add the new card to the map
  addToken(cardDiv);
  updateDexModAttributes();
  addInputForPlayers();
  updateTokenPositions();
  
  // Optional: Clear the form fields after submission
  document.getElementById('characterName').value = '';
  document.getElementById('characterDex').value = '';
  document.getElementById('characterImage').value = '';
  document.getElementById('characterType').value = 'empty';
  document.getElementById('createCharacterForm').classList.add('element-hidden');
});

document.getElementById('mapSelect').addEventListener('change', function() {
  var selectedOption = this.options[this.selectedIndex];
  var imageUrl = selectedOption.value; // Get the value from the selected option
  document.getElementById('draggable-image').src = imageUrl; // Update the image source
});
