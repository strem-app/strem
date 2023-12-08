const awaitSelector = (selector, rootNode, fallbackDelay) => new Promise((resolve, reject) => {
    try {
        const root = rootNode || document
        const ObserverClass = MutationObserver || WebKitMutationObserver || null
        const mutationObserverSupported = typeof ObserverClass === 'function'
        let observer
        const stopWatching = () => {
            if (observer) {
                if (mutationObserverSupported) {
                    observer.disconnect()
                } else {
                    clearInterval(observer)
                }
                observer = null
            }
        }
        const findAndResolveElements = () => {
            const allElements = root.querySelectorAll(selector)
            if (allElements.length === 0) return
            const newElements = []
            const attributeForBypassing = 'data-awaitselector-resolved'
            allElements.forEach((el, i) => {
                if (typeof el[attributeForBypassing] === 'undefined') {
                    allElements[i][attributeForBypassing] = ''
                    newElements.push(allElements[i])
                }
            })
            if (newElements.length > 0) {
                stopWatching()
                resolve(newElements)
            }
        }
        if (mutationObserverSupported) {
            observer = new ObserverClass(mutationRecords => {
                const nodesWereAdded = mutationRecords.reduce(
                    (found, record) => found || (record.addedNodes && record.addedNodes.length > 0),
                    false
                )
                if (nodesWereAdded) {
                    findAndResolveElements()
                }
            })
            observer.observe(root, {
                childList: true,
                subtree: true,
            })
        } else {
            observer = setInterval(findAndResolveElements, fallbackDelay || 250)
        }
        findAndResolveElements()
    } catch (exception) {
        reject(exception)
    }
});

const watchAwaitSelector = (callback, selector, rootNode, fallbackDelay) => {
    (function awaiter(continueWatching = true) {
        if (continueWatching === false) return
        awaitSelector(selector, rootNode, fallbackDelay)
            .then(callback)
            .then(awaiter)
    }())
}