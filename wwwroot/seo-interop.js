// SEO and Metadata Management JavaScript Functions

window.setPageMetadata = (metadata) => {
    // Update page title
    if (metadata.title) {
        document.title = metadata.title;
    }

    // Update or create meta tags
    updateMetaTag('description', metadata.description);
    updateMetaTag('keywords', metadata.keywords);
    updateMetaTag('image', metadata.imageUrl);
    updateMetaTag('url', metadata.url);

    // Update canonical URL
    if (metadata.url) {
        updateCanonicalUrl(metadata.url);
    }
};

window.setJsonLd = (jsonString) => {
    // Remove existing JSON-LD scripts
    const existingScripts = document.querySelectorAll('script[type="application/ld+json"]');
    existingScripts.forEach(script => script.remove());

    // Add new JSON-LD script
    const script = document.createElement('script');
    script.type = 'application/ld+json';
    script.textContent = jsonString;
    document.head.appendChild(script);
};

window.setOpenGraph = (metadata) => {
    // Open Graph meta tags
    updateMetaTag('og:title', metadata.title, 'property');
    updateMetaTag('og:description', metadata.description, 'property');
    updateMetaTag('og:image', metadata.imageUrl, 'property');
    updateMetaTag('og:url', metadata.url, 'property');
    updateMetaTag('og:type', 'website', 'property');
    updateMetaTag('og:site_name', 'YourNoteBook', 'property');
};

window.setTwitterCard = (metadata) => {
    // Twitter Card meta tags
    updateMetaTag('twitter:card', 'summary_large_image', 'name');
    updateMetaTag('twitter:title', metadata.title, 'name');
    updateMetaTag('twitter:description', metadata.description, 'name');
    updateMetaTag('twitter:image', metadata.imageUrl, 'name');
    updateMetaTag('twitter:url', metadata.url, 'name');
    updateMetaTag('twitter:site', '@YourNoteBook', 'name');
};

// Helper function to update or create meta tags
function updateMetaTag(name, content, attribute = 'name') {
    if (!content) return;

    let metaTag = document.querySelector(`meta[${attribute}="${name}"]`);
    
    if (!metaTag) {
        metaTag = document.createElement('meta');
        metaTag.setAttribute(attribute, name);
        document.head.appendChild(metaTag);
    }
    
    metaTag.setAttribute('content', content);
}

// Helper function to update canonical URL
function updateCanonicalUrl(url) {
    if (!url) return;

    let canonicalLink = document.querySelector('link[rel="canonical"]');
    
    if (!canonicalLink) {
        canonicalLink = document.createElement('link');
        canonicalLink.setAttribute('rel', 'canonical');
        document.head.appendChild(canonicalLink);
    }
    
    canonicalLink.setAttribute('href', url);
}


// Track page views for analytics
window.trackPageView = (pageTitle, pagePath) => {
    if (typeof gtag !== 'undefined') {
        gtag('config', 'G-VM01Q3R43D', {
            page_title: pageTitle,
            page_location: window.location.href,
            page_path: pagePath
        });
    }
};

// Track custom events
window.trackEvent = (eventName, parameters = {}) => {
    if (typeof gtag !== 'undefined') {
        gtag('event', eventName, parameters);
    }
};
