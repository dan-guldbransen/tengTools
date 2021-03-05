import React, { Fragment } from 'react';

const Navigation = ({links=[], contentLink=null }) => {
    let menuRef = React.createRef();
    function toggleMenu() {
        menuRef.current.classList.toggle('navbar__menu--show');
    }
    const additionClass = contentLink && contentLink.attributes ? contentLink.attributes.cssValue : null;
    const selectedClass = contentLink && contentLink.isSelected ? 'navbar__link--selected' : '';
    const hasChildrenClass = links.length > 0 ? 'has-children' : null;
    const hasNameOrChildren = link => link.name || (link.links || []).length > 0;

    return (
    <Fragment>
        {!contentLink ? (
            <a className="navbar__link--block navbar__icon--menu navbar__icon" onClick={toggleMenu}></a>
        ) : (
            <Fragment>
                <a className={`navbar__link ${selectedClass} ${hasChildrenClass || '' } ${additionClass || ''}`} 
                    href={contentLink.url||'#'} dangerouslySetInnerHTML={{__html: contentLink.name}} >
                </a>
                {links.length > 0 &&
                    <i className="navbar__icon--caret-right navbar__icon navbar__icon--open" onClick={toggleMenu}></i>
                }
            </Fragment>
        )}

        { links.length > 0 &&
            <ul className="navbar__menu" ref={menuRef}>
                <div className="navbar__menu-header">
                    {!contentLink ? (
                        <span className="navbar__icon navbar__icon--close" onClick={toggleMenu}></span>
                    ) : (
                        <Fragment>
                            <i className="navbar__icon--caret-left navbar__icon" onClick={toggleMenu}></i>
                            <span className="navbar__title" onClick={toggleMenu} dangerouslySetInnerHTML={{__html: contentLink.name}}></span>
                        </Fragment>
                    )}
                </div>
                {links.length > 0 && links.map((link, i) => hasNameOrChildren(link) &&
                    <li className="navbar__item" key={link.url}>
                        <Navigation links={link.links} contentLink={link}/>
                    </li>
                )}
            </ul>
        }
        
    </Fragment>
)}

export default Navigation;