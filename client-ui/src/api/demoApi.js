// Demo API for testing authentication
// This simulates API responses for development/testing purposes

const demoUsers = [
  {
    id: 1,
    name: 'John Doe',
    email: 'john@example.com',
    username: 'johndoe'
  },
  {
    id: 2,
    name: 'Jane Smith',
    email: 'jane@example.com',
    username: 'janesmith'
  }
];

// Simulate API delay
const delay = (ms) => new Promise(resolve => setTimeout(resolve, ms));

export const demoApi = {
  login: async (emailOrUsername, password) => {
    await delay(1000); // Simulate network delay
    
    // Simple validation
    if (!emailOrUsername || !password) {
      throw new Error('Email/Username and password are required');
    }
    
    if (password.length < 6) {
      throw new Error('Password must be at least 6 characters');
    }
    
    // Find user by email or username
    const user = demoUsers.find(u => 
      u.email === emailOrUsername || u.username === emailOrUsername
    );
    
    if (!user) {
      throw new Error('Invalid credentials');
    }
    
    // Simulate successful login
    return {
      accessToken: 'demo-jwt-token-' + Date.now(),
      refreshToken: 'demo-refresh-token-' + Date.now(),
      profile: user
    };
  },
  
  register: async (payload) => {
    await delay(1000);
    
    const { name, email, password } = payload;
    
    if (!name || !email || !password) {
      throw new Error('All fields are required');
    }
    
    if (password.length < 6) {
      throw new Error('Password must be at least 6 characters');
    }
    
    if (demoUsers.find(u => u.email === email)) {
      throw new Error('Email already exists');
    }
    
    // Simulate successful registration
    return {
      message: 'Registration successful'
    };
  },
  
  forgotPassword: async (email) => {
    await delay(1000);
    
    if (!email) {
      throw new Error('Email is required');
    }
    
    const user = demoUsers.find(u => u.email === email);
    if (!user) {
      throw new Error('Email not found');
    }
    
    return {
      message: 'Password reset email sent'
    };
  }
};
