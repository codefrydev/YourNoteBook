// Global variable to hold the Firestore instance.
let db;

/**
 * Initializes Firebase and Firestore with the provided config.
 * @param {object} config - Your Firebase configuration object.
 * @returns {boolean} - Returns true when initialization is complete.
 */
function initializeFirebase(config) {
    const app = firebase.initializeApp(config);
    db = firebase.firestore();
    return true;
}

/**
 * Saves a document to the specified collection.
 * @param {string} parent - The name of the collection where the document should be added.
 * @param {object} document - An object representing the document data.
 * @returns {object} - An object with success status and the new document ID if successful.
 */
async function saveDocument(parent, document) {
    try {
        const ref = await db.collection(parent).add({
            ...document,
            created: firebase.firestore.FieldValue.serverTimestamp()
        });
        return {success: true, id: ref.id};
    } catch (error) {
        return {success: false, error: error.message};
    }
}

/**
 * Retrieves a document from the specified collection by its ID.
 * @param {string} parent - The name of the collection.
 * @param {string} documentId - The document ID.
 * @returns {object|null} - The document data (with its ID) or null if not found.
 */
async function getDocument(parent, documentId) {
    try {
        const doc = await db.collection(parent).doc(documentId).get();
        if (doc.exists) {
            const data = doc.data();
            // Convert Firestore timestamp to milliseconds if available.
            data.created = data.created ? data.created.toMillis() : Date.now();
            data.id = doc.id;
            return data;
        } else {
            return null;
        }
    } catch (error) {
        console.error('Error retrieving document:', error);
        return null;
    }
}

/**
 * Retrieves all documents from the specified collection.
 * @param {string} parent - The name of the collection.
 * @returns {object|null} - The document data (with its ID) or null if not found.
 */
async function getAllDocuments(parent) {
    try {
        const snapshot = await db.collection(parent).get();
        const documents = [];

        snapshot.forEach(doc => {
            const data = doc.data();
            data.created = data.created ? data.created.toMillis() : Date.now();
            data.id = doc.id;
            documents.push(data);
        });

        return documents;
    } catch (error) {
        console.error('Error retrieving documents:', error);
        return [];
    }
}
/**
 * Updates an existing document in the specified collection.
 * @param {string} parent - The name of the collection.
 * @param {string} documentId - The document ID to update.
 * @param {object} document - An object with the updated document data.
 * @returns {object} - An object with success status.
 */
async function updateDocument(parent, documentId, document) {
    try {
        await db.collection(parent).doc(documentId).update(document);
        return {success: true};
    } catch (error) {
        return {success: false, error: error.message};
    }
}

/**
 * Deletes a document from the specified collection.
 * @param {string} parent - The name of the collection.
 * @param {string} documentId - The document ID to delete.
 * @returns {object} - An object with success status.
 */
async function deleteDocument(parent, documentId) {
    try {
        await db.collection(parent).doc(documentId).delete();
        return {success: true};
    } catch (error) {
        return {success: false, error: error.message};
    }
}

// firebaseFolderInterop.js

/**
 * Retrieves a folder document along with its subcollections ("notes" and "shortcuts")
 * from the "folders" collection.
 * @param parentPath
 * @param {string} folderId - The document ID of the folder.
 * @param collectionOne
 * @param collectionTwo
 * @returns {object|null} - The folder data including subcollections, or null if not found.
 */
async function getFolderWithDetails(parentPath, folderId,collectionOne,collectionTwo) {
    try {
        // Retrieve the folder document
        const folderDoc = await db.collection(parentPath).doc(folderId).get();
        if (!folderDoc.exists) {
            return null;
        }
        let folder = folderDoc.data();
        folder.id = folderDoc.id;
        folder.created = folder.created ? folder.created.toMillis() : Date.now();

        // Retrieve the subcollection "collectionOne"
        const shortcutsSnapshot = await db.collection(parentPath)
            .doc(folderId)
            .collection(collectionOne)
            .get();
        const shortcuts = [];
        shortcutsSnapshot.forEach(doc => {
            let data = doc.data();
            data.id = doc.id;
            shortcuts.push(data);
        });
        folder.shortcuts = shortcuts;

        // Retrieve the subcollection "collecctioTwo"
        const notesSnapshot = await db.collection(parentPath)
            .doc(folderId)
            .collection(collecctioTwo)
            .get();
        const notes = [];
        notesSnapshot.forEach(doc => {
            let data = doc.data();
            data.id = doc.id;
            // Convert timestamp if needed
            data.created = data.created ? data.created.toMillis() : Date.now();
            data.modified = data.modified ? data.modified.toMillis() : Date.now();
            notes.push(data);
        });
        folder.notes = notes;

        return folder;
    } catch (error) {
        console.error("Error retrieving folder with details:", error);
        return null;
    }
}

// Expose the function for JS interop.
window.initializeFirebase = initializeFirebase;
window.saveDocument = saveDocument;
window.getDocument = getDocument;
window.getAllDocuments = getAllDocuments; 
window.updateDocument = updateDocument;
window.deleteDocument = deleteDocument;
window.getFolderWithDetails = getFolderWithDetails; 

