export const setCookie = (name, value, options = {}) => {
  const defaultOptions = {
    path: "/",
    secure: true,
    sameSite: "strict",
  };

  const cookieOptions = { ...defaultOptions, ...options };
  let cookieString = `${name}=${encodeURIComponent(value)}`;

  if (cookieOptions.maxAge) {
    cookieString += `; max-age=${cookieOptions.maxAge}`;
  }
  if (cookieOptions.path) {
    cookieString += `; path=${cookieOptions.path}`;
  }
  if (cookieOptions.secure) {
    cookieString += "; secure";
  }
  if (cookieOptions.sameSite) {
    cookieString += `; samesite=${cookieOptions.sameSite}`;
  }

  document.cookie = cookieString;
};

export const getCookie = (name) => {
  const match = document.cookie.match(new RegExp("(^| )" + name + "=([^;]+)"));
  return match ? decodeURIComponent(match[2]) : null;
};

export const deleteCookie = (name) => {
  document.cookie = name + "=; Max-Age=0; path=/";
};
