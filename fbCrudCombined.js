// Folder Operations
window.fbAddFolder = async (folder) => {
    const docRef = db.collection('folders').doc(folder.Id);
    await docRef.set({
        Name: folder.Name,
        Created: firebase.firestore.Timestamp.fromDate(new Date(folder.Created))
    });
    return docRef.id;
};

window.fbGetAllFolders = async () => {
    const snapshot = await db.collection('folders').get();
    return snapshot.docs.map(doc => ({
        Id: doc.id,
        ...doc.data(),
        Created: doc.data().Created?.toDate().toISOString()
    }));
};

window.fbGetFolderById = async (id) => {
    const doc = await db.collection('folders').doc(id).get();
    return doc.exists ? {
        Id: doc.id,
        ...doc.data(),
        Created: doc.data().Created?.toDate().toISOString()
    } : null;
};

window.fbUpdateFolder = async (folder) => {
    await db.collection('folders').doc(folder.Id).update({
        Name: folder.Name
    });
};

window.fbDeleteFolder = async (id) => {
    await db.collection('folders').doc(id).delete();
};

// Note Operations
window.fbAddNote = async (note) => {
    const docRef = db.collection('notes').doc(note.Id);
    await docRef.set({
        Title: note.Title,
        Content: note.Content,
        Created: firebase.firestore.Timestamp.fromDate(new Date(note.Created)),
        Modified: firebase.firestore.Timestamp.fromDate(new Date(note.Modified)),
        FolderId: note.FolderId,
        Tags: note.Tags.map(t => t.Id)
    });
    return docRef.id;
};

window.fbGetAllNotes = async () => {
    const snapshot = await db.collection('notes').get();
    return Promise.all(snapshot.docs.map(async doc => {
        const data = doc.data();
        const tags = await Promise.all(data.Tags.map(async tagId => {
            const tagDoc = await db.collection('tags').doc(tagId).get();
            return { Id: tagDoc.id, ...tagDoc.data() };
        }));

        return {
            Id: doc.id,
            ...data,
            Created: data.Created?.toDate().toISOString(),
            Modified: data.Modified?.toDate().toISOString(),
            Tags: tags
        };
    }));
};

window.fbGetNoteById = async (id) => {
    const doc = await db.collection('notes').doc(id).get();
    if (!doc.exists) return null;

    const data = doc.data();
    const tags = await Promise.all(data.Tags.map(async tagId => {
        const tagDoc = await db.collection('tags').doc(tagId).get();
        return { Id: tagDoc.id, ...tagDoc.data() };
    }));

    return {
        Id: doc.id,
        ...data,
        Created: data.Created?.toDate().toISOString(),
        Modified: data.Modified?.toDate().toISOString(),
        Tags: tags
    };
};

window.fbUpdateNote = async (note) => {
    await db.collection('notes').doc(note.Id).update({
        Title: note.Title,
        Content: note.Content,
        Modified: firebase.firestore.Timestamp.fromDate(new Date()),
        Tags: note.Tags.map(t => t.Id)
    });
};

window.fbDeleteNote = async (id) => {
    await db.collection('notes').doc(id).delete();
};

// Tag Operations
window.fbAddTag = async (tag) => {
    const docRef = db.collection('tags').doc(tag.Id);
    await docRef.set({ Name: tag.Name });
    return docRef.id;
};

window.fbGetAllTags = async () => {
    const snapshot = await db.collection('tags').get();
    return snapshot.docs.map(doc => ({
        Id: doc.id,
        ...doc.data()
    }));
};

window.fbGetTagById = async (id) => {
    const doc = await db.collection('tags').doc(id).get();
    return doc.exists ? { Id: doc.id, ...doc.data() } : null;
};

window.fbUpdateTag = async (tag) => {
    await db.collection('tags').doc(tag.Id).update({ Name: tag.Name });
};

window.fbDeleteTag = async (id) => {
    await db.collection('tags').doc(id).delete();
};

// Shortcut Operations
window.fbAddShortcut = async (shortcut) => {
    const docRef = db.collection('shortcuts').doc(shortcut.Id);
    await docRef.set({
        Action: shortcut.Action,
        Keys: shortcut.Keys,
        Description: shortcut.Description,
        Category: db.doc('categories/' + shortcut.Category.Id),
        Created: firebase.firestore.Timestamp.fromDate(new Date(shortcut.Created)),
        Modified: firebase.firestore.Timestamp.fromDate(new Date(shortcut.Modified)),
        FolderId: shortcut.FolderId
    });
    return docRef.id;
};

window.fbGetAllShortcuts = async () => {
    const snapshot = await db.collection('shortcuts').get();
    return Promise.all(snapshot.docs.map(async doc => {
        const data = doc.data();
        const category = await data.Category?.get();

        return {
            Id: doc.id,
            ...data,
            Created: data.Created?.toDate().toISOString(),
            Modified: data.Modified?.toDate().toISOString(),
            Category: {
                Id: category?.id,
                Name: category?.data()?.Name
            }
        };
    }));
};

window.fbGetShortcutById = async (id) => {
    const doc = await db.collection('shortcuts').doc(id).get();
    if (!doc.exists) return null;

    const data = doc.data();
    const category = await data.Category?.get();

    return {
        Id: doc.id,
        ...data,
        Created: data.Created?.toDate().toISOString(),
        Modified: data.Modified?.toDate().toISOString(),
        Category: {
            Id: category?.id,
            Name: category?.data()?.Name
        }
    };
};

window.fbUpdateShortcut = async (shortcut) => {
    await db.collection('shortcuts').doc(shortcut.Id).update({
        Action: shortcut.Action,
        Keys: shortcut.Keys,
        Description: shortcut.Description,
        Category: db.doc('categories/' + shortcut.Category.Id),
        Modified: firebase.firestore.Timestamp.fromDate(new Date())
    });
};

window.fbDeleteShortcut = async (id) => {
    await db.collection('shortcuts').doc(id).delete();
};

// Category Operations
window.fbAddCategory = async (category) => {
    const docRef = db.collection('categories').doc(category.Id);
    await docRef.set({ Name: category.Name });
    return docRef.id;
};

window.fbGetAllCategories = async () => {
    const snapshot = await db.collection('categories').get();
    return snapshot.docs.map(doc => ({
        Id: doc.id,
        ...doc.data()
    }));
};

window.fbGetCategoryById = async (id) => {
    const doc = await db.collection('categories').doc(id).get();
    return doc.exists ? { Id: doc.id, ...doc.data() } : null;
};

window.fbUpdateCategory = async (category) => {
    await db.collection('categories').doc(category.Id).update({ Name: category.Name });
};

window.fbDeleteCategory = async (id) => {
    await db.collection('categories').doc(id).delete();
};

// Relationship Queries
window.fbGetNotesByFolder = async (folderId) => {
    const snapshot = await db.collection('notes')
        .where('FolderId', '==', folderId)
        .get();

    return Promise.all(snapshot.docs.map(async doc => {
        const data = doc.data();
        const tags = await Promise.all(data.Tags.map(async tagId => {
            const tagDoc = await db.collection('tags').doc(tagId).get();
            return { Id: tagDoc.id, ...tagDoc.data() };
        }));

        return {
            Id: doc.id,
            ...data,
            Created: data.Created?.toDate().toISOString(),
            Modified: data.Modified?.toDate().toISOString(),
            Tags: tags
        };
    }));
};

window.fbGetShortcutsByFolder = async (folderId) => {
    const snapshot = await db.collection('shortcuts')
        .where('FolderId', '==', folderId)
        .get();

    return Promise.all(snapshot.docs.map(async doc => {
        const data = doc.data();
        const category = await data.Category?.get();

        return {
            Id: doc.id,
            ...data,
            Created: data.Created?.toDate().toISOString(),
            Modified: data.Modified?.toDate().toISOString(),
            Category: {
                Id: category?.id,
                Name: category?.data()?.Name
            }
        };
    }));
};